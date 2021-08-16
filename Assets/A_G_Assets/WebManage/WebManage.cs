using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Threading;
//using System.Net.WebSockets;
using System;
using System.IO;
using NativeWebSocket;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public class WebManage : MonoBehaviour
{ //use websocket 81
  //
    public bool MoveToWaiting = false;
    public bool TryToJoinFriend = false;
    public uint TryToJoinFriendVar = 0;

    public bool GiveMapDataFriendlies = false;

    public bool GivenMapData = false;

    stringC rs = new stringC();
    stringC FilePath = new stringC();
    
    byte[] byteArr;

    public stringC JsonSendS = new stringC();

    public stringC JsonReceiveS = new stringC();

    public byte[] RawCharDat;

    public stringC StringRawCharDat = new stringC();

    public int MessageType = 1;
    
    public uint MatchType = 0; //WebManage.WManage.MatchType
    public uint TeamOrder = 100;
    //local is 0
    //random online is 1
    //WebManage.WManage.NeedToSendCharsRandomChar
    public bool NeedToSendCharsRandomChar = false;

    public bool sentData = false;

    public bool StartMatchTToggle1 = false;
    //if StartMatchTToggle1 == true{SceneManager.LoadScene("BattleType1StartTrue")} <-- main idea to start match type 1
    public class stringC
    {
        public string s;

        public stringC()
        {
            s = "";
        }
        public stringC(string i)
        {
            s = i;
        }
    }

    stringC result = new stringC();

    bool connected = false;

    public string ErrorMessage = "";

    public bool ErrorAquired = false;

    public static WebManage WManage;

    public uint id = 0; //0 means nothing

    public bool GetLoginID = false;

    public bool wait = false;

    public string LoginUsername; // only allow numbers and letters, else people can do weird sql injection attacks
    public string LoginPassword; // only allow numbers and letters, else people can do weird sql injection attacks

    public string Username;

    public string email; //force remove ; ' -- /*   */   xp_ 

    public bool CreateNewAccountBool = false;

    public bool RandomMatch = false;

    
    public bool SendMapDataBat1Bool = false;
    public bool GetMapDataBat1Bool = false;

    public bool FoundMatch = false;

    IEnumerator logicWS;

    int TurnToPick = 0;


    int TeamCountGetter(stringC FileString)
    {
        localMatchIntermediateCS.LocalMatchSaveMapData data = JsonConvert.DeserializeObject<localMatchIntermediateCS.LocalMatchSaveMapData>(FileString.s);

        int teamCount = 0;

        if(data.BlueChar.Count != 0) teamCount++;
        if (data.RedChar.Count != 0) teamCount++;
        if (data.YellowChar.Count != 0) teamCount++;
        if (data.GreenChar.Count != 0) teamCount++;
        if (data.PurpleChar.Count != 0) teamCount++;

        return teamCount;
    }

    //Uri u = new Uri("ws://174.95.213.15:81/");
    WebSocket cws = null;
    ArraySegment<byte> buf = new ArraySegment<byte>(new byte[1024]);

    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath
            + "/Garbage/"))
        {
            FileInfo fileInfo = new FileInfo(Application.persistentDataPath
+ "/Garbage/");
            fileInfo.Directory.Create(); //make not existing file path
        }


        if (!Directory.Exists(Application.persistentDataPath
                         + "/TmpMapDat/"))
        {
            FileInfo fileInfo = new FileInfo(Application.persistentDataPath
         + "/TmpMapDat/");

            fileInfo.Directory.Create(); //make not existing file path
        }



        if (WManage == null)
        {
            WManage = this;

            DontDestroyOnLoad(WManage);

            Connect();

            SetupOverallOnMessage();
        }
        else if (WManage != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

    }

    void SetError(string Message) //
    {
        ErrorAquired = true;
        ErrorMessage = Message;
    }

    void SetupOverallOnMessage() //so I don't need to deal with event system this is an all in 1
    {

        WManage.cws.OnMessage += (bytes) =>
        {
            if (WebManage.WManage.MessageType == 1)
            {
                WebManage.WManage.rs.s = System.Text.Encoding.UTF8.GetString(bytes);

                if (WebManage.WManage.rs.s == "")
                {
                    WebManage.WManage.rs.s = "0";
                }
            }
            else if (WebManage.WManage.MessageType == 2)
            {
                Debug.Log("Got File of [bytes]: " + bytes.Length);

                File.WriteAllBytes(WebManage.WManage.FilePath.s, bytes);

                WManage.sentData = false;
            }
            else if (WebManage.WManage.MessageType == 3)
            {
                WebManage.WManage.byteArr = bytes;

                WManage.sentData = false;
            }

            WManage.sentData = false;
        };

    }

    void GetMessS(stringC rs2)
    {
        sentData = true;

        WebManage.WManage.MessageType = 1;

        WebManage.WManage.rs = rs2;
    }

    void GetAndSaveFile(stringC FilePath2, stringC rs2)
    {
        sentData = true;

        WebManage.WManage.MessageType = 2;

        WebManage.WManage.FilePath = FilePath2;
        WebManage.WManage.rs = rs2;
    }

    void GetRawBytes(byte[] byteArr2)
    {
        sentData = true;

        WebManage.WManage.MessageType = 3;

        WebManage.WManage.byteArr = byteArr2;
    }

    public void LoadBackToMainFromLogin()
    {
        if (WManage.GetLoginID == false && WManage.CreateNewAccountBool == false)
            WManage.ErrorAquired = false;

        WManage.CreateNewAccountBool = false;

        WManage.GetLoginID = false;

        StaticDataMapMaker.controlObj.LoadMain();

    }

    public void LoadRandomMatchBackToMain() //TODO: make it clear from ID avaible from random match pool when you quit - and test [need to test now] - remove back when found match is true - TODO
    {//or make a confirm button :shrug:
        if (FoundMatch == false)
        {
            WManage.RandomMatch = false;



            StaticDataMapMaker.controlObj.LoadMain();
        }
    }

    IEnumerator logicM()
    {
        //try
        //{

        //await cws.ConnectAsync(u, CancellationToken.None);
        if (cws.State == WebSocketState.Open)
        {
            Debug.Log("connected");

            while (cws.State == WebSocketState.Open)
            {

                if (id == 0)
                { //get tmp id - reserved from 1 bill and onward

                    cws.SendText("TID");

                    GetMessS(result);

                    while (sentData == true) { yield return null; };

                    Debug.Log("CreateRUID");

                    id = UInt32.Parse(result.s);

                    Debug.Log("Got: " + id);

                    Username = id.ToString();
                    ///


                }
                else if (GetLoginID == true)
                {

                    ///
                    //get login id if you login 1 - less than random nums by a large sum - AskAndGetFullId

                    cws.SendText("FID");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    Debug.Log("Got: " + result.s);

                    cws.SendText(email);

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    Debug.Log("Got: " + result.s);

                    if ("Y" == result.s)
                    { //email is in data base
                        cws.SendText(LoginPassword);

                        GetMessS(result);
                        while (sentData == true) { yield return null; };

                        Debug.Log("Got: " + result.s);

                        if ("Y" == result.s)
                        { //password is alined with email in data base

                            cws.SendText("Y");

                            GetMessS(result);
                            while (sentData == true) { yield return null; };

                            id = UInt32.Parse(result.s); //get id after password info

                            cws.SendText("Y");

                            GetMessS(result);
                            while (sentData == true) { yield return null; };

                            Username = result.s;

                            cws.SendText("Y");

                        }
                        else
                        {
                            SetError(result.s);
                            //password is wrong
                            //send email to person in question that someone has failed atempt at log-ing into their account
                        }
                    }
                    else
                    {
                        SetError(result.s);
                        //email is not in data-base
                    }
                    GetLoginID = false;
                    ///
                }

                else if (CreateNewAccountBool == true)
                {

                    ///
                    //CreateNewAccount();
                    cws.SendText("CID");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    Debug.Log("Got: " + result.s);

                    cws.SendText(email);

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    Debug.Log("Got: " + result.s);

                    if ("Y" == result.s)
                    {//5 means that the email is not used and is real - you can continue to create new account 
                        
                        cws.SendText(LoginUsername);
                        
                        Debug.Log("sentData Create Acc");

                        GetMessS(result);

                        Debug.Log("GotData Create Acc");

                        while (sentData == true) { yield return null; };

                        Debug.Log("Got: " + result.s);

                        if ("Y" == result.s)
                        {
                            cws.SendText(LoginPassword);

                            GetMessS(result);
                            while (sentData == true) { yield return null; };

                            Debug.Log("Got: " + result.s + "Done");

                            id = UInt32.Parse(result.s); //get id after password info is sent for your new account number
                        }
                        else
                        {

                            SetError(result.s);

                        }
                    }
                    else
                    {
                        SetError(result.s);
                        //email is used/not-real

                    }
                    CreateNewAccountBool = false;
                    ///
                }
                else if (RandomMatch == true)
                {

                    Debug.Log("RandomMatchText");

                    StartMatchTToggle1 = false;

                    cws.SendText("RM");

                    //GetMessS(result);
                    //while (sentData == true) { yield return null; };

                    //query for all waiting players and server picks 2 random ones - until then you wait for a true return and it signs you to continue
                    //cws.SendText(LoginUsername);

                    TeamOrder = 100;

                    StaticDataMapMaker.controlObj.LoadMapDatPath = "";

                    string PairedUp = "f";

                    while (PairedUp == "f" || RandomMatch == false)
                    {
                        GetMessS(result);
                        while (sentData == true) { yield return null; };

                        PairedUp = result.s;

                        cws.SendText(result.s);
                    }

                    // need to fetch and pass on username of guy you are fighting
                    FoundMatch = true;

                    //
                    RandomMatch = false;

                    stringC RandomMapPath = new stringC(Application.persistentDataPath + "/RR1.txt"); //named RR1.txt
                    stringC RBFName = new stringC();

                    //Debug.Log("File Rand Get Path 1");
                    GetAndSaveFile(RandomMapPath, RBFName);
                    //Debug.Log("File Rand Get Path 2");
                    while (sentData == true) { yield return null; };
                    
                    FileStream stream = null;
                    bool randb = true;
                    while (randb) {
                        try
                        {
                            stream = File.Open(RandomMapPath.s, FileMode.Open);
                            randb = false;
                        }
                        catch
                        {
                            Debug.Log("Still Writing");
                        }
                        yield return null;
                    }
                    stream.Close();

                    StaticDataMapMaker.controlObj.LoadMapDatPath = RandomMapPath.s;
                    
                    cws.SendText("h");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    MatchType = UInt32.Parse(result.s); //error here so I remember to read email and add both things I listed with semicolon - last 3 emails

                    cws.SendText("h");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    TeamOrder = UInt32.Parse(result.s); //pass team order - missing semi colon to remind to add to server

                    cws.SendText("h");

                    Debug.Log("team Order first time sent");
                }
                else if(GiveMapDataFriendlies == true)
                {
                    GiveMapDataFriendlies = false;

                    cws.SendText("GMDF");
                    //
                    JsonSendS.s = File.ReadAllText(StaticDataMapMaker.controlObj.LoadMapDatPath);
                    //
                    GetMessS(result);
                    while (sentData == true) { yield return null; }

                    cws.SendText(TeamCountGetter(JsonSendS).ToString()); // player count

                    GetMessS(result);
                    while (sentData == true) { yield return null; }

                    cws.SendText(JsonSendS.s.Length.ToString()); // map send

                    GetMessS(result);
                    while (sentData == true) { yield return null; }

                    cws.SendText(JsonSendS.s); // map send

                    GetMessS(result);
                    while (sentData == true) { yield return null; };
                    
                    result.s = "f";
                    while(result.s == "f")
                    {
                        GetMessS(result);
                        while (sentData == true) { yield return null; };

                    }

                    MatchType = 1;

                    GetMessS(JsonReceiveS); // GOT MAP
                    while (sentData == true) { yield return null; };

                    cws.SendText("f");

                    GetMessS(result); // GOT MAP
                    while (sentData == true) { yield return null; };

                    TeamOrder = UInt32.Parse(result.s);

                    StaticDataMapMaker.controlObj.LoadMapDatPath = "UOT";

                    FoundMatch = true;
                    //now loadmap -
                    
                    cws.SendText("f");

                }
                else if (TryToJoinFriend == true)
                {
                    GiveMapDataFriendlies = false;

                    cws.SendText("JFMS");

                    GetMessS(result);
                    while (sentData == true) { yield return null; }

                    cws.SendText(TryToJoinFriendVar.ToString());

                    GetMessS(result);
                    while (sentData == true) { yield return null; }

                    if(result.s == "BAD")
                    {
                        MoveToWaiting = false; 
                        Debug.Log("No Player To Join");
                    }
                    else
                    {
                        MoveToWaiting = true;
                        Debug.Log("Ready to join a Player");

                        while (result.s == "g")
                        {
                            GetMessS(result); // GOT MAP
                            while (sentData == true) { yield return null; };
                        }

                        MatchType = 1;

                        GetMessS(JsonReceiveS); // GOT MAP
                        while (sentData == true) { yield return null; };

                        cws.SendText("f");

                        GetMessS(result); // GOT MAP
                        while (sentData == true) { yield return null; };

                        TeamOrder = UInt32.Parse(result.s);


                        StaticDataMapMaker.controlObj.LoadMapDatPath = "UOT";
                        FoundMatch = true;
                        //now loadmap -

                        cws.SendText("f");

                        //now loadmap -
                    }

                }

                else if (NeedToSendCharsRandomChar == true) 
                {
                    Debug.Log("SentCharDat");

                    NeedToSendCharsRandomChar = false;

                    cws.SendText("NTSCRR");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    id = UInt32.Parse(result.s);

                    
                    cws.SendText(JsonSendS.s.Length.ToString());

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    //stringC RandomCharSentPath = new stringC(Application.persistentDataPath
                    //+ "/Garbage/TMPSEND.txt"); //named RR1.txt

                    cws.SendText( JsonSendS.s );

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                }
                else if (SendMapDataBat1Bool == true)
                {
                    //SendMapDataFunc();
                    cws.SendText("SMDBAT1");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    //Debug.Log(JsonSendS.s.Length);

                    cws.SendText(JsonSendS.s.Length.ToString());

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    cws.SendText(JsonSendS.s);

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    SendMapDataBat1Bool = false;
                }
                //else if (GetMapDataBat1Bool == true) //request map data bool
                //{
                    //GetMapDataFunc();


                //    GetMapDataBat1Bool = false;
                //}

                else //keep connection
                {
                    //Debug.Log("LogDump");
                    //Say hello - by getting ID of the server side so you don't cheat my ID system :anger:
                    cws.SendText("h");

                    GetMessS(result);
                    //Debug.Log("S1");
                    while (sentData == true) { yield return null; };
                    //Debug.Log(result.s);
                    //   Debug.Log(result.s);
                    id = UInt32.Parse(result.s); //get id after password info is sent for your new account number 
                    
                    cws.SendText("h");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    MatchType = UInt32.Parse(result.s); //error here so I remember to read email and add both things I listed with semicolon - last 3 emails

                    cws.SendText("h");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    TeamOrder = UInt32.Parse(result.s); //pass team order - missing semi colon to remind to add to server

                    cws.SendText("h");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    if (result.s == "MR")
                    {
                        Debug.Log("MR start");

                        //do stuff if 

                    }

                    else if(result.s == "CLR")
                    {
                        Debug.Log("CLR start");

                        cws.SendText("h");

                        GetMessS(StringRawCharDat);
                        while (sentData == true) { yield return null; };

                        Debug.Log("CharDat: "+ StringRawCharDat.s);
                        //you now have to load char array data after reciving the bytes and deserializing with RawCharDat var

                        StartMatchTToggle1 = true;
                    }
                    cws.SendText("h");

                    GetMessS(result);
                    while (sentData == true) { yield return null; };

                    if (result.s == "NTSMD")
                    {
                        
                        cws.SendText("h");

                        GetMessS(JsonReceiveS);
                        while (sentData == true) { yield return null; };
                        
                        GivenMapData = true;
                        WebManage.WManage.GetMapDataBat1Bool = false;

                    }
                    cws.SendText("h");



                    //TODO: constant poll for usernmae to send - maybe? maybe not? - slower if I do, so I think I will just update it before a match and send it there
                    //Username = id.ToString();
                }

            }

        }
        else
        {
            Debug.Log("failed to connect");
        }



        //}
        //catch (Exception e)
        //{
        //    Debug.Log("ISSUE: " + e.Message);
        //    yield return null;
        //}
        yield return null;
    }

    async void Connect()
    {
        cws = new WebSocket("ws://142.113.123.191:81/", "null");


        cws.OnOpen += () =>
        {
            logicWS = logicM();
            StartCoroutine(logicWS);
        };

        cws.Connect();//may need await

    }

    IEnumerator WaitToConnectAgain()
    {
        wait = true;
        int frame = 0;
        while (frame < 60)
        {
            frame++;
            yield return null;
        }

        Connect();
        wait = false;
        yield return null;
    }

    void Update()
    {

#if !UNITY_WEBGL || UNITY_EDITOR

        cws.DispatchMessageQueue(); //handle 1 at a time? - also why am I sending more than 1 at a time? - make it wait until result  after yeild

#endif

        if ((cws.State == WebSocketState.Closed || cws.State == WebSocketState.Closing) && wait == false)
        {

            if (RandomMatch == true || CreateNewAccountBool == true || FoundMatch == true || GetLoginID == true)
            {
                StaticDataMapMaker.controlObj.LoadMain();
            }

            if (logicWS != null) 
            {
                GivenMapData = false;
                StartMatchTToggle1 = false;
                NeedToSendCharsRandomChar = false;
                sentData = false;
                ErrorAquired = false;
                GetLoginID = false;
                id = 0;
                wait = false;
                LoginUsername = "";
                LoginPassword = "";
                Username = "";
                email = "";
                CreateNewAccountBool = false;
                RandomMatch = false;
                SendMapDataBat1Bool = false;
                GetMapDataBat1Bool = false;
                FoundMatch = false;
                TeamOrder = 100;
                StaticDataMapMaker.controlObj.LoadMapDatPath = "";

                StopCoroutine(logicWS);
                StaticDataMapMaker.controlObj.LoadMain();
            }
            
            
            id = 0;
            IEnumerator wtca = WaitToConnectAgain();
            StartCoroutine(wtca);

        }
    }

    private async void OnApplicationQuit()
    {
        await cws.Close();
    }

    void OnDestroy()
    {
        if (cws != null)
        {
            if (cws.State == WebSocketState.Open)
            {
                OnApplicationQuit();
            }
        }
    }
}
