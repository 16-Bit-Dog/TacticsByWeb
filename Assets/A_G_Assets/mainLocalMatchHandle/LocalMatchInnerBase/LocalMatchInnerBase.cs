using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;
using Newtonsoft.Json;

//using UnityEngine.JSONSerializeModule;

public class LocalMatchInnerBase : MonoBehaviour
{

    int[] ColorCount = { 0, 0, 0, 0, 0 }; //1 for each color type - linked to team/overlap for the 5 colors that exist

    Dictionary<int, int> SudoTurnOrder = new Dictionary<int, int>(); //Team (0-5, not overlap including, so add 1 if I link to overlap) linked to turn order
    Dictionary<int, int> SudoTurnOrderInv = new Dictionary<int, int>(); //Team (0-5, not overlap including, so add 1 if I link to overlap) linked to turn order

    public Material blue;
    public Material red;
    public Material green;
    public Material yellow;
    public Material purple;

    List<Material> matArr = new List<Material>();

    public static LocalMatchInnerBase LMIB;

    bool SelectCharBlock = false;

    [Serializable]
    public class CharDat
    {

        public int PosX;
        public int PosY;

        public int team; //based on overlap - // 0 is null, 1 is blue, 2 is red, 3 is yellow, 4 is green, 5 is purple

        public int Hp;
        public int Atk;
        public int Def;
        public int Mov;
        public int PW; //Pri weapon id
        public int SW; //Secondary weapon id
        public int CH; //Cosmetic hat id
        public int CB; //Cosmetic body id

        public string name; //
    }
    [Serializable]
    public class CharDatL
    {

        public List<CharDat> C = new List<CharDat>();

    }

    public class MapMakerVars
    {
        public GameObject Tile = new GameObject();
        public int TileId = 6;
        public int Overlap = 0; // 0 is null, 1 is blue, 2 is red, 3 is yellow, 4 is green, 5 is purple
        public GameObject CharObj = new GameObject();
        public GameObject HatObj = new GameObject();
        public GameObject BodyObj = new GameObject();

        public GameObject WeaponObj = new GameObject();


    }

    public MapMakerVars[][] TilesArray;



    public Dictionary<Tuple<int, int>, CharDat> UnorderedCharMess = new Dictionary<Tuple<int, int>, CharDat>(); //unordered Chars in dictionary of position - <x,y>,

    int TotalOverlaps = 0;

    public string CharFilePath;

    Tuple<int, int> selectionSpots;
    int selectionOverlap;

    public GameObject CharNamesSelect;
    public GameObject CharOBJECT;

    public GameObject HoverInfo;
    public TMP_Text HoverInfoText;
    bool PermaHoverInfoTextOff = false;

    Tuple<int, int> HoverTupleStore;

    string HoverInfoTMP = "";

    private UnityTemplateProjects.SimpleCameraController.CameraState CamS;

    public TMP_Text TileInfoHover;
    public GameObject TileInfoHoverScrollView;
    bool TileInfoHoverToggle = true;

    public void ToggleTileInfoHover()
    {
        TileInfoHoverScrollView.SetActive(!TileInfoHoverScrollView.activeSelf);
        LMIB.TileInfoHoverToggle = !LMIB.TileInfoHoverToggle;

    }

    public void TakeRawWebCharDataAddToMap()
    {
        CharDatL CDatTmp;

        LMIB.UnorderedCharMess.Clear();

        CDatTmp = JsonConvert.DeserializeObject<CharDatL>(WebManage.WManage.StringRawCharDat.s);

        for(int i = 0; i < CDatTmp.C.Count; i++)
        {
            LMIB.UnorderedCharMess[Tuple.Create(CDatTmp.C[i].PosY,CDatTmp.C[i].PosX)] = CDatTmp.C[i];
        }
    }

    public void PassDataToIntermediateCS()
    {

        localMatchIntermediateCS.LMICS.TilesArray = new localMatchIntermediateCS.MapMakerVarsDat[LMIB.TilesArray.GetLength(0)][];


        for (int i = 0; i < LMIB.TilesArray.GetLength(0); i++)
        {

            localMatchIntermediateCS.LMICS.TilesArray[i] = new localMatchIntermediateCS.MapMakerVarsDat[LMIB.TilesArray[i].GetLength(0)];


            for (int ii = 0; ii < LMIB.TilesArray[i].GetLength(0); ii++)
            {
                localMatchIntermediateCS.LMICS.TilesArray[i][ii] = new localMatchIntermediateCS.MapMakerVarsDat();
                localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId = LMIB.TilesArray[i][ii].TileId;
                localMatchIntermediateCS.LMICS.TilesArray[i][ii].Overlap = LMIB.TilesArray[i][ii].Overlap;
            }
        }


        foreach (Tuple<int, int> i in LMIB.UnorderedCharMess.Keys)
        {
            localMatchIntermediateCS.LMICS.tmpCharDat = new localMatchIntermediateCS.CharDat();

            localMatchIntermediateCS.LMICS.tmpCharDat.PosX = LMIB.UnorderedCharMess[i].PosX;
            localMatchIntermediateCS.LMICS.tmpCharDat.PosY = LMIB.UnorderedCharMess[i].PosY;
            localMatchIntermediateCS.LMICS.tmpCharDat.team = LMIB.UnorderedCharMess[i].team;
            localMatchIntermediateCS.LMICS.tmpCharDat.Hp = LMIB.UnorderedCharMess[i].Hp;
            localMatchIntermediateCS.LMICS.tmpCharDat.Atk = LMIB.UnorderedCharMess[i].Atk;
            localMatchIntermediateCS.LMICS.tmpCharDat.Def = LMIB.UnorderedCharMess[i].Def;
            localMatchIntermediateCS.LMICS.tmpCharDat.Mov = LMIB.UnorderedCharMess[i].Mov;
            localMatchIntermediateCS.LMICS.tmpCharDat.SW = LMIB.UnorderedCharMess[i].SW;
            localMatchIntermediateCS.LMICS.tmpCharDat.CH = LMIB.UnorderedCharMess[i].CH;
            localMatchIntermediateCS.LMICS.tmpCharDat.CB = LMIB.UnorderedCharMess[i].CB;
            localMatchIntermediateCS.LMICS.tmpCharDat.name = LMIB.UnorderedCharMess[i].name;
            localMatchIntermediateCS.LMICS.tmpCharDat.PW = LMIB.UnorderedCharMess[i].PW;

            switch (LMIB.UnorderedCharMess[i].team)
            {
                case 1:

                    localMatchIntermediateCS.LMICS.blueC.Add(localMatchIntermediateCS.LMICS.tmpCharDat);
                    //localMatchIntermediateCS.blueC[localMatchIntermediateCS.blueC.Count]. = LMIB.UnorderedCharMess[i].;
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat = localMatchIntermediateCS.LMICS.tmpCharDat;

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinPri = PriWLookup.PWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxPri = PriWLookup.PWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinSec = SecWLookup.SWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxSec = SecWLookup.SWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Atk2 = SecWLookup.SWLO.Atk[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Def2 = SecWLookup.SWLO.Def[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];


                    break;

                case 2:




                    localMatchIntermediateCS.LMICS.redC.Add(localMatchIntermediateCS.LMICS.tmpCharDat);
                    //localMatchIntermediateCS.redC[localMatchIntermediateCS.redC.Count]. = LMIB.UnorderedCharMess[i].;
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat = localMatchIntermediateCS.LMICS.tmpCharDat;

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinPri = PriWLookup.PWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxPri = PriWLookup.PWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinSec = SecWLookup.SWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxSec = SecWLookup.SWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Atk2 = SecWLookup.SWLO.Atk[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Def2 = SecWLookup.SWLO.Def[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];


                    break;

                case 3:



                    localMatchIntermediateCS.LMICS.yellowC.Add(localMatchIntermediateCS.LMICS.tmpCharDat);
                    //localMatchIntermediateCS.yellowC[localMatchIntermediateCS.yellowC.Count]. = LMIB.UnorderedCharMess[i].;
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat = localMatchIntermediateCS.LMICS.tmpCharDat;

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinPri = PriWLookup.PWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxPri = PriWLookup.PWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinSec = SecWLookup.SWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxSec = SecWLookup.SWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Atk2 = SecWLookup.SWLO.Atk[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Def2 = SecWLookup.SWLO.Def[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];


                    break;

                case 4:



                    localMatchIntermediateCS.LMICS.greenC.Add(localMatchIntermediateCS.LMICS.tmpCharDat);
                    //localMatchIntermediateCS.greenC[localMatchIntermediateCS.greenC.Count]. = LMIB.UnorderedCharMess[i].;
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat = localMatchIntermediateCS.LMICS.tmpCharDat;

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinPri = PriWLookup.PWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxPri = PriWLookup.PWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinSec = SecWLookup.SWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxSec = SecWLookup.SWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Atk2 = SecWLookup.SWLO.Atk[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Def2 = SecWLookup.SWLO.Def[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];


                    break;

                case 5:


                    localMatchIntermediateCS.LMICS.purpleC.Add(localMatchIntermediateCS.LMICS.tmpCharDat);
                    //localMatchIntermediateCS.purpleC[localMatchIntermediateCS.greenC.Count]. = LMIB.UnorderedCharMess[i].;
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat = localMatchIntermediateCS.LMICS.tmpCharDat;

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinPri = PriWLookup.PWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxPri = PriWLookup.PWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.PW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMinSec = SecWLookup.SWLO.MinRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.RngMaxSec = SecWLookup.SWLO.MaxRng[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];

                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Atk2 = SecWLookup.SWLO.Atk[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];
                    localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.Def2 = SecWLookup.SWLO.Def[localMatchIntermediateCS.LMICS.TilesArray[i.Item1][i.Item2].CDat.SW];




                    break;

            }

        }


    }

    void MatchType0Set()
    {
        if (LMIB.UnorderedCharMess.Count == LMIB.TotalOverlaps)
        {
            PassDataToIntermediateCS();
            SceneManager.LoadScene("LocalMatchStart");
        }

    }

    void MatchType1Set()
    {

        if (LMIB.UnorderedCharMess.Count == (LMIB.ColorCount[LMIB.SudoTurnOrderInv[Convert.ToInt32(WebManage.WManage.TeamOrder)]]) ) //only your characters were filled - do second check that only your chars are filled - by having seperate var check off when you fill them? TODO : ?
        {


            CharDatL BinaryToSendChars = new CharDatL();
            //BinaryToSendChars.C.Resize(LMIB.ColorCount[LMIB.SudoTurnOrderInv[Convert.ToInt32(WebManage.WManage.TeamOrder)]]);

            foreach (Tuple<int, int> i in LMIB.UnorderedCharMess.Keys)
            {
                BinaryToSendChars.C.Add(LMIB.UnorderedCharMess[i]);
            }

            WebManage.WManage.JsonSendS.s = JsonConvert.SerializeObject(BinaryToSendChars);

            Debug.Log(WebManage.WManage.JsonSendS.s);

            WebManage.WManage.NeedToSendCharsRandomChar = true;

            SceneManager.LoadScene("Match1WaitMenu"); //load wating place for everyone... ? use email idea

        }
        
    }

    public void StartLocalMatch()
    {
        if (WebManage.WManage.MatchType == 0)
        {
            MatchType0Set(); //local match setup start
        }

        else if (WebManage.WManage.MatchType == 1)
        {//
            MatchType1Set(); //Random Match Randos setup start
        }

    }



    public void LoadCharFromFile()
    {
        CharNamesSelect.SetActive(false);

        LMIB.UnorderedCharMess[LMIB.selectionSpots] = new CharDat();
        //load CharFilePath THEN do rest of loading
        
        StaticDataMapMaker.SaveCharData data = JsonConvert.DeserializeObject<StaticDataMapMaker.SaveCharData>(File.ReadAllText(CharFilePath));


        LMIB.SelectCharBlock = false;

        //TODO: retrive HP, atk, move, ect values once I add them
        LMIB.UnorderedCharMess[LMIB.selectionSpots].PW = data.pW;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].SW = data.sW;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].CH = data.cH;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].CB = data.cB;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].name = data.name;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].team = selectionOverlap;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].PosX = LMIB.selectionSpots.Item2;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].PosY = LMIB.selectionSpots.Item1;
        LMIB.UnorderedCharMess[LMIB.selectionSpots].Atk = PriWLookup.PWLO.Atk[LMIB.UnorderedCharMess[LMIB.selectionSpots].PW];
        LMIB.UnorderedCharMess[LMIB.selectionSpots].Def = PriWLookup.PWLO.Def[LMIB.UnorderedCharMess[LMIB.selectionSpots].PW];
        LMIB.UnorderedCharMess[LMIB.selectionSpots].Hp = PriWLookup.PWLO.Hp[LMIB.UnorderedCharMess[LMIB.selectionSpots].PW];
        LMIB.UnorderedCharMess[LMIB.selectionSpots].Mov = PriWLookup.PWLO.Mov[LMIB.UnorderedCharMess[LMIB.selectionSpots].PW];


        //TODO:
        LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].CharObj = Instantiate(CharOBJECT, new Vector3(4 * LMIB.selectionSpots.Item2, -19, 4 * LMIB.selectionSpots.Item1), Quaternion.identity);
        LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].HatObj = Instantiate(HatLookup.HLO.Hat[data.cH], new Vector3(0, 0, 0), Quaternion.identity);
        //LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].BodyObj = Instantiate(BodyLookup.BLO.Body[data.cH], new Vector3(0,0,0), Quaternion.identity);

        LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].WeaponObj = Instantiate(PriWLookup.PWLO.PriWeapons[data.pW], new Vector3(0, 0, 0), Quaternion.identity);

        ParentConstraint pc1 = LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].WeaponObj.AddComponent<ParentConstraint>();
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].CharObj.transform.Find(ConstantCharObject.CCObj.WHandString); 

        constraintSource.weight = 1;

        pc1.AddSource(constraintSource);
        pc1.constraintActive = true;
//        pc1.rotationAxis = Axis.None;
        pc1.SetTranslationOffset(0, new Vector3(-1.0f, 0.0f, 0.0f));
        pc1.SetRotationOffset(0, new Vector3(270.0f, 90.0f, 0.0f));


        //SetColor
        Renderer[] ComponentR;
        ComponentR = LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].CharObj.GetComponentsInChildren<Renderer>();
        ComponentR[0].material = LMIB.matArr[LMIB.UnorderedCharMess[LMIB.selectionSpots].team - 1];

        Transform head;

        head = LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].CharObj.transform.Find(ConstantCharObject.CCObj.headString);

        LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].HatObj.transform.SetParent(head, false);

        //Fix size
        LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].CharObj.transform.localScale += new Vector3(-0.5f, -0.5f, -0.5f);

        localMatchIntermediateCS.LMICS.ChooseAndSetAnim(PriWLookup.PWLO.MainAnimNum[LMIB.UnorderedCharMess[LMIB.selectionSpots].PW], LMIB.TilesArray[LMIB.selectionSpots.Item1][LMIB.selectionSpots.Item2].CharObj.GetComponent<Animator>());

    }


    public void TogglePermaInfoOnOff()
    {

        LMIB.PermaHoverInfoTextOff = !LMIB.PermaHoverInfoTextOff;

    }

    void ShowStats(int y, int x)
    {
        if(LMIB.UnorderedCharMess.ContainsKey( Tuple.Create(y,x) ) && LMIB.PermaHoverInfoTextOff == false || LMIB.HoverTupleStore != null && LMIB.PermaHoverInfoTextOff == false) //second part never allows to turn off - this changes in the real game with a red x button to cancel out and such
        {
            if(LMIB.HoverTupleStore == null || Tuple.Create(y, x) != LMIB.HoverTupleStore && LMIB.UnorderedCharMess.ContainsKey(Tuple.Create(y, x)))
            {

                LMIB.HoverTupleStore = Tuple.Create(y, x);
                LMIB.HoverInfoTMP = LMIB.UnorderedCharMess[LMIB.HoverTupleStore].name + ":\nHP: " + LMIB.UnorderedCharMess[LMIB.HoverTupleStore].Hp + "\nAttack: " + LMIB.UnorderedCharMess[LMIB.HoverTupleStore].Atk + "\nDefence: " + LMIB.UnorderedCharMess[LMIB.HoverTupleStore].Def + "\nMovement: " + LMIB.UnorderedCharMess[LMIB.HoverTupleStore].Mov+"\nPrimary Weapon: "+ PriWLookup.PWLO.Name[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].PW] + "\nMin-Range-Primary: " + PriWLookup.PWLO.MinRng[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].PW] + "\nMax-Range-Primary: " + PriWLookup.PWLO.MaxRng[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].PW] + "\nSecondary Weapon: " + SecWLookup.SWLO.Name[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].SW] + "\nSecondary Atk: "+ SecWLookup.SWLO.Atk[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].SW] + "\nSecondary Def: "+ SecWLookup.SWLO.Def[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].SW] + "\nMin-Range-Secondary: " + SecWLookup.SWLO.MinRng[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].SW] + "\nMan-Range-Secondary: " + SecWLookup.SWLO.MaxRng[LMIB.UnorderedCharMess[LMIB.HoverTupleStore].SW];

            }   
            LMIB.HoverInfo.SetActive(true);
            LMIB.HoverInfoText.text = HoverInfoTMP;
        }
        else
        {
            HoverInfoTMP = "";
            HoverTupleStore = null;

            LMIB.HoverInfo.SetActive(false);

        }

        if (TileInfoHoverToggle)
        {
            StaticDataMapMaker.controlObj.DrawTileStats(TileLookUp.TLU.Tiles[LMIB.TilesArray[y][x].TileId], LMIB.TileInfoHover);

        }


    }

    void checkForOverlap(int y, int x)
    {
#if UNITY_EDITOR
//        Debug.Log("y    " + y);
//        Debug.Log("x    " + x);
#endif
        switch (LMIB.TilesArray[y][x].Overlap)
        {
            case 0:
                break;

            default:
                if (WebManage.WManage.MatchType == 0)
                {
                    if (TilesArray[y][x].CharObj != null)
                    {
                        Destroy(TilesArray[y][x].CharObj);
                    }

                    //       LMIB.UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); <-- logic, but I do else where for when I select a file
                    LMIB.selectionOverlap = LMIB.TilesArray[y][x].Overlap;
                    LMIB.selectionSpots = Tuple.Create(y, x);
                    LMIB.SelectCharBlock = true;
                    CharNamesSelect.SetActive(true);
                    //TODO: unhide char selector scroll view
                    
                }
                else if (WebManage.WManage.MatchType == 1 && SudoTurnOrder[LMIB.TilesArray[y][x].Overlap-1] == WebManage.WManage.TeamOrder)
                {
                    if (TilesArray[y][x].CharObj != null)
                    {
                        Destroy(TilesArray[y][x].CharObj);
                    }

                    //       LMIB.UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); <-- logic, but I do else where for when I select a file
                    LMIB.selectionOverlap = LMIB.TilesArray[y][x].Overlap;
                    LMIB.selectionSpots = Tuple.Create(y, x);
                    LMIB.SelectCharBlock = true;
                    CharNamesSelect.SetActive(true);
                    //TODO: unhide char selector scroll view
                }
                break;

                /*
                case 1:
                    UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); //add to blue Char
                    break;

                case 2:
                    UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); //add to red Char
                    break;

                case 3:
                    UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); //add to yellow Char
                    break;

                case 4:
                    UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); //add to green Char
                    break;

                case 5:
                    UnorderedCharMess[Tuple.Create(y, x)] = LoadCharFromFile(TilesArray[y][x].Overlap); //add to purple Char
                    break;
                */
        }





    }

    void FillCharInArrSetup(int Put)
    {
        if (Put != 0)
        {
            LMIB.ColorCount[Put - 1] += 1; //overlap is team +1 due to 0 being nothing
        }
    }

    void FillSudoTurnOrder()
    {

        for(int i = 0; i < LMIB.ColorCount.Length; i++)
        {
            if (LMIB.ColorCount[i] != 0) // color exists means that you add to dictionary a new element in the dic - inverse and regular
            {
                LMIB.SudoTurnOrder[i] = LMIB.SudoTurnOrder.Count;
                LMIB.SudoTurnOrderInv[ LMIB.SudoTurnOrder[i] ] = i;

                //Debug.Log(SudoTurnOrder[i]);
                //Debug.Log(SudoTurnOrderInv[SudoTurnOrder[i]]);

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        LMIB = this;

        LMIB.matArr.Add(LMIB.blue);
        LMIB.matArr.Add(LMIB.red);
        LMIB.matArr.Add(LMIB.green);
        LMIB.matArr.Add(LMIB.yellow);
        LMIB.matArr.Add(LMIB.purple);

        StaticDataMapMaker.controlObj.LoadMapDat = false;

        FileStream file = null;

        try
        {
            file = File.Open(StaticDataMapMaker.controlObj.LoadMapDatPath, FileMode.Open);
            file.Close();
        }
        catch
        {
            Debug.Log("Still Writing - DEAD OR - UOT");
        }

        StaticDataMapMaker.SaveMapData data;

        if (StaticDataMapMaker.controlObj.LoadMapDatPath != "UOT")
        {
            data = JsonConvert.DeserializeObject<StaticDataMapMaker.SaveMapData>(File.ReadAllText(StaticDataMapMaker.controlObj.LoadMapDatPath));
        }
        else
        {
            data = JsonConvert.DeserializeObject<StaticDataMapMaker.SaveMapData>(WebManage.WManage.JsonReceiveS.s);
        }
        
        StaticDataMapMaker.controlObj.MapName = data.MapName;
        StaticDataMapMaker.controlObj.MapWidth = data.MapWidth;
        StaticDataMapMaker.controlObj.MapHeight = data.MapHeight;


        //if (data.HighestHpWin == null)
        //{
            localMatchIntermediateCS.LMICS.HighestHpWin = data.HighestHpWin;
            localMatchIntermediateCS.LMICS.HighestHpWinTurnLimit = data.HighestHpWinTurnLimit;
            localMatchIntermediateCS.LMICS.MonumentWin = data.MonumentWin;
            localMatchIntermediateCS.LMICS.MonumentTurnLimit = data.MonumentTurnLimit;
        //}
        //else
        //{
        //            localMatchIntermediateCS.LMICS.HighestHpWin = false;
        //            localMatchIntermediateCS.LMICS.HighestHpWinTurnLimit = 30;
        //            localMatchIntermediateCS.LMICS.MonumentWin = false;
        //            localMatchIntermediateCS.LMICS.MonumentTurnLimit = 30;
        //        }
//        Debug.Log(data.HighestHpWin);

        LMIB.TilesArray = new MapMakerVars[StaticDataMapMaker.controlObj.MapHeight][];
        for (int i = 0; i < LMIB.TilesArray.GetLength(0); i++)
        {
            TilesArray[i] = new MapMakerVars[StaticDataMapMaker.controlObj.MapWidth];
            for (int ii = 0; ii < LMIB.TilesArray[i].GetLength(0); ii++)
            {
                LMIB.TilesArray[i][ii] = new MapMakerVars();
                LMIB.TilesArray[i][ii].Overlap = data.TilesArrayID[i][ii].Overlap;
                LMIB.TilesArray[i][ii].TileId = data.TilesArrayID[i][ii].TileId;
                LMIB.TilesArray[i][ii].Tile = new GameObject();
                LMIB.TilesArray[i][ii].CharObj = new GameObject();
                LMIB.TilesArray[i][ii].Tile = Instantiate(TileLookUp.TLU.Tiles[LMIB.TilesArray[i][ii].TileId].Obj, new Vector3(4 * ii, -20, 4 * i), Quaternion.identity);
                LMIB.TilesArray[i][ii].CharObj = Instantiate(TileLookUp.TLU.Tiles[LMIB.TilesArray[i][ii].Overlap].Obj, new Vector3(4 * ii, -17, 4 * i), Quaternion.identity);

                LMIB.FillCharInArrSetup(LMIB.TilesArray[i][ii].Overlap);


            }
        }

        LMIB.FillSudoTurnOrder();

        Debug.Log(SudoTurnOrderInv[0]);

        DrawGrid.GridLines.UpdateDrawGridWidthBasedMap();
        DrawGrid.GridLines.UpdateDrawGridHeightBasedMap();
        DrawGrid.GridLines.UpdateGridDraw();

        CamS = Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState;

#if UNITY_EDITOR
#endif


        for (int i = 0; i < LMIB.TilesArray.GetLength(0); i++)
        {
            for (int ii = 0; ii < LMIB.TilesArray[i].GetLength(0); ii++)
            {
                if (LMIB.TilesArray[i][ii].Overlap != 0)
                {
                    TotalOverlaps += 1;
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (CamS.x < 0)
        {

            CamS.x = 1;

        }

        if (CamS.x > 4 * TilesArray[0].GetLength(0))
        {

            CamS.x = 4 * TilesArray[0].GetLength(0) - 1;

        }


        if (CamS.z < 0)
        {

            CamS.z = 1;

        }

        if (CamS.z > 4 * TilesArray.GetLength(0))
        {

            CamS.z = 4 * TilesArray.GetLength(0) - 1;

        }

        //for now always check for char overlap - Vector3 tmpVec3 = LightRayToCoords.LRC.GetRayCamMouseXYZ();
        Vector3 tmpVec3 = LightRayToCoords.LRC.GetRayCamMouseXYZ();

        int V3y = Convert.ToInt32((Math.Abs(tmpVec3.z / 4)));
        int V3x = Convert.ToInt32((Math.Abs(tmpVec3.x / 4)));

        if (V3x < 0 || V3x > TilesArray[0].GetLength(0))
        {
            V3x = 0;
        }
        if (V3y < 0 || V3y > TilesArray.GetLength(0))
        {
            V3y = 0;
        }

        if (LMIB.SelectCharBlock == false && Input.GetMouseButton(0))
        {
            if (tmpVec3 != new Vector3(-10000, -10000, -10000))
            {
                checkForOverlap(V3y, V3x);

            }
        }
        else
        {

            

            //mostly wait for clicking with scroll view 

        }

        ShowStats(V3y, V3x);
    }
}
