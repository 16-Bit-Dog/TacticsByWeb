using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.Animations;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public class LocalMatchStart : MonoBehaviour
{


    public void BackToMainMenuAndSave()
    {
        
        if (WebManage.WManage.MatchType == 0) 
        {
            LMS.SaveMapStateAll();
        }
  //      Destroy(localMatchIntermediateCS.LMICS);

        localMatchIntermediateCS.LMICS = null;

        SceneManager.LoadScene("Main");

    }

    public struct LineDrawer //main logic utilized from stack overflow... I found this... and realized this was a good implementation
    {
        private LineRenderer lineRenderer;
        private float lineSize;

        public LineDrawer(float lineSize = 0.2f)
        {
            GameObject lineObj = new GameObject("LineObj");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            //Particles/Additive
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;
        }

        private void init(float lineSize = 0.2f)
        {
            if (lineRenderer == null)
            {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                //Particles/Additive
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                this.lineSize = lineSize;
            }
        }

        //Draws lines through the provided vertices
        public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null)
            {
                init(0.2f);
            }

            //Set color
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            //Set width
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;

            //Set line count which is 2
            lineRenderer.positionCount = 2;

            //Set the postion of both two lines
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        public void Destroy()
        {
            if (lineRenderer != null)
            {
                UnityEngine.Object.Destroy(lineRenderer.gameObject);
            }
        }
    }

    List<Action> TSAFuncs = new List<Action>();

    LineDrawer ArrowLineDrawer;


    public LocalMatchStart LMS;

    public bool SetYourColorThing = false;

    public TMP_Text WhichColorTurn;
    
    public TMP_Text YourColorTurn; //limited to multiplayer
    
    public GameObject LMICSObjMidLoad;

    int Winner = 0;

    public localMatchIntermediateCS.CharDat SelectedChar;

    bool currentlyFighting = false;

    int CXPos = 0;
    int CYPos = 0;

    public int currentTurn = 0;

    public int CurrentTeamTurn = 0;

    public int TeamCount = 0;

    int MonumentValBlue = 0;
    int MonumentValRed = 0;
    int MonumentValYellow = 0;
    int MonumentValGreen = 0;
    int MonumentValPurple = 0;


    public GameObject HoverInfo;
    public TMP_Text HoverInfoText;
    public GameObject HoverInfoCloseButton;
    bool PermaHoverInfoTextOff = false;
    Tuple<int, int> HoverTupleStore;
    string HoverInfoTMP = "";


    public GameObject ActionMenuObj;
    public GameObject CloseActionMenuObj;
    public GameObject CloseActionAtkObj;
    
    public GameObject ActionAbilityInfoObj;
    public TMP_Text ActionAbilityInfoText;
    public GameObject CloseActionAbilityObj;

    public GameObject Attack1;
    public GameObject Attack2;
    public GameObject Ability1;
    public GameObject Ability2;
    public GameObject Move;

    public GameObject TileInfoObj;
    public TMP_Text TileInfoObjText;
    public GameObject TileInfoToggle;
    bool HideTileInfo;
    
    public class MovePhaseC
    {

        public int MovePhase = 0;

    }

    MovePhaseC MovePhase = new MovePhaseC(); //int since I may need many states other than on/off
    
    Dictionary<Tuple<int,int>,GameObject> TmpMoveTiles = new Dictionary<Tuple<int, int>, GameObject>();
    bool CalcMoveDone = true;

    Dictionary<Tuple<int, int>, GameObject> TmpAtkTiles = new Dictionary<Tuple<int, int>, GameObject>();
    bool attackPrep = false;
    public GameObject AttackPrepScreenAttacker;
    public GameObject AttackPrepScreenDefender;
    public TMP_Text AttackPrepScreenTextAttacker;
    public TMP_Text AttackPrepScreenTextDefender;
    public GameObject StartToAttackButton;

    Tuple<int, int> SelectedAttackTarget;


    public GameObject CloseActionRotateObjBackButton;
    public TMP_Text TurnCountText;

    public GameObject WinConditionObj;
    public TMP_Text WinCondition;


    Dictionary<Tuple<int, int>, GameObject> TmpAbilityTiles = new Dictionary<Tuple<int, int>, GameObject>();
    //bool AbilityMoveDone = true;

    public void SendMapDataNetworkBat1()
    {

        WebManage.WManage.JsonSendS.s = LMS.SaveMapStateAllLogic();

        WebManage.WManage.SendMapDataBat1Bool = true;

    }

    public string SaveMapStateAllLogic()
    {
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath
                     + "/MapsLocalSave/");

        if (!fileInfo.Directory.Exists) fileInfo.Directory.Create(); //make not existing file path

        localMatchIntermediateCS.LocalMatchSaveMapData data = new localMatchIntermediateCS.LocalMatchSaveMapData();

        //easy vars from LMICS fill
        data.SaveMapName = localMatchIntermediateCS.LMICS.SaveMapName;
        data.HighestHpWin = localMatchIntermediateCS.LMICS.HighestHpWin;
        data.HighestHpWinTurnLimit = localMatchIntermediateCS.LMICS.HighestHpWinTurnLimit;
        data.MonumentWin = localMatchIntermediateCS.LMICS.MonumentWin;
        data.MonumentTurnLimit = localMatchIntermediateCS.LMICS.MonumentTurnLimit;
        //

        //easy vars from local match start

        data.Winner = LMS.Winner;
        data.CalcMoveDone = LMS.CalcMoveDone;
        data.MovePhase = LMS.MovePhase.MovePhase;
        data.CXPos = LMS.CXPos;
        data.CYPos = LMS.CYPos;
        data.currentlyFighting = LMS.currentlyFighting;
        data.currentTurn = LMS.currentTurn;
        data.CurrentTeamTurn = LMS.CurrentTeamTurn;

Debug.Log(LMS.TeamCount);
        data.TeamCount = LMS.TeamCount;
        data.MonumentValBlue = LMS.MonumentValBlue;
        data.MonumentValRed = LMS.MonumentValRed;
        data.MonumentValYellow = LMS.MonumentValYellow;
        data.MonumentValGreen = LMS.MonumentValGreen;
        data.MonumentValPurple = LMS.MonumentValPurple;
        data.PermaHoverInfoTextOff = LMS.PermaHoverInfoTextOff;
        data.HoverInfoTMP = LMS.HoverInfoTMP;
        data.HideTileInfo = LMS.HideTileInfo;
        data.attackPrep = LMS.attackPrep;

        //Window State vars
        data.IsHoverInfoActive = LMS.HoverInfo.activeSelf;
        data.IsHoverInfoCloseButtonActive = LMS.HoverInfoCloseButton.activeSelf;
        data.IsActionMenuObjActive = LMS.ActionMenuObj.activeSelf;
        data.IsCloseActionMenuObjActive = LMS.CloseActionMenuObj.activeSelf;
        data.IsCloseActionAtkObjActive = LMS.CloseActionAtkObj.activeSelf;
        data.IsActionAbilityInfoObjActive = LMS.ActionAbilityInfoObj.activeSelf;
        data.IsCloseActionAbilityObjActive = LMS.CloseActionAbilityObj.activeSelf;
        data.IsAttack1Active = LMS.Attack1.activeSelf;
        data.AIsttack2Active = LMS.Attack2.activeSelf;
        data.IsAbility1Active = LMS.Ability1.activeSelf;
        data.IsAbility2Active = LMS.Ability2.activeSelf;
        data.IsMoveActive = LMS.Move.activeSelf;
        data.IsTileInfoObjActive = LMS.TileInfoObj.activeSelf;
        data.IsTileInfoToggleActive = LMS.TileInfoToggle.activeSelf;
        data.IsAttackPrepScreenAttackerActive = LMS.AttackPrepScreenAttacker.activeSelf;
        data.IsAttackPrepScreenDefenderActive = LMS.AttackPrepScreenDefender.activeSelf;
        data.IsStartToAttackButtonActive = LMS.StartToAttackButton.activeSelf;
        data.IsCloseActionRotateObjBackButtonActive = LMS.CloseActionRotateObjBackButton.activeSelf;
        data.IsWinConditionObjActive = LMS.WinConditionObj.activeSelf;

        ///Hard to convert var - selected char, tmp tiles, selected atk target, and hover tuple store:
        if (LMS.SelectedChar != null)
        {
            data.SelectedCharX = LMS.SelectedChar.PosX;
            data.SelectedCharY = LMS.SelectedChar.PosY;
        }
        else
        {
            data.SelectedCharX = -1;
            data.SelectedCharY = -1;
        }

        data.HoverTupleStore = LMS.HoverTupleStore; //x y pos for tuples and list

        data.SelectedAttackTarget = LMS.SelectedAttackTarget;

        //fill tmp action tiles with position vars for where to show action item
        foreach (Tuple<int, int> i in LMS.TmpAbilityTiles.Keys)
        {
            data.TmpAbilityTiles.Add(i);
        }
        foreach (Tuple<int, int> i in LMS.TmpMoveTiles.Keys)
        {
            data.TmpMoveTiles.Add(i);
        }
        foreach (Tuple<int, int> i in LMS.TmpAtkTiles.Keys)
        {
            data.TmpAtkTiles.Add(i);
        }
        //
        //Load Tile Data:
        data.TilesArray = new localMatchIntermediateCS.MapMakerVarsSaveDat[localMatchIntermediateCS.LMICS.TilesArray.GetLength(0)][];
        for (int i = 0; i < localMatchIntermediateCS.LMICS.TilesArray.GetLength(0); i++)
        {
            data.TilesArray[i] = new localMatchIntermediateCS.MapMakerVarsSaveDat[localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0)];

            for (int ii = 0; ii < localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0); ii++)
            {
                data.TilesArray[i][ii] = new localMatchIntermediateCS.MapMakerVarsSaveDat();

                data.TilesArray[i][ii].TileId = localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId;

                if (localMatchIntermediateCS.LMICS.TilesArray[i][ii].CDat != null)
                {
                    data.TilesArray[i][ii].CDatX = localMatchIntermediateCS.LMICS.TilesArray[i][ii].CDat.PosX;
                    data.TilesArray[i][ii].CDatY = localMatchIntermediateCS.LMICS.TilesArray[i][ii].CDat.PosY;
                }
                else
                {
                    data.TilesArray[i][ii].CDatX = -1;
                    data.TilesArray[i][ii].CDatY = -1;
                }
            }

        }
        //
        //LoadCharacters
        LoadCharInfoLocalMatchStart(localMatchIntermediateCS.LMICS.blueC, data.BlueChar);
        LoadCharInfoLocalMatchStart(localMatchIntermediateCS.LMICS.redC, data.RedChar);
        LoadCharInfoLocalMatchStart(localMatchIntermediateCS.LMICS.yellowC, data.YellowChar);
        LoadCharInfoLocalMatchStart(localMatchIntermediateCS.LMICS.greenC, data.GreenChar);
        LoadCharInfoLocalMatchStart(localMatchIntermediateCS.LMICS.purpleC, data.PurpleChar);

        return JsonConvert.SerializeObject(data);
    }

    public void SaveMapStateAll()
    {
        File.WriteAllText(Application.persistentDataPath
                     + "/MapsLocalSave/" + localMatchIntermediateCS.LMICS.SaveMapName + ".dat", LMS.SaveMapStateAllLogic());
    }

    void LoadCharsInto(List<localMatchIntermediateCS.CharDat> CharData, List<localMatchIntermediateCS.InFlightCharData> CharDataIn)
    {
        for (int i = 0; i < CharDataIn.Count; i++)
        {
            Debug.Log("Char Passed Load Into: " + i.ToString());

            localMatchIntermediateCS.CharDat tmp = new localMatchIntermediateCS.CharDat();

            CharData.Add(tmp);

            CharData[i].HasTurn = CharDataIn[i].HasTurn;
            CharData[i].MovedAlready = CharDataIn[i].MovedAlready;
            CharData[i].Ability1 = CharDataIn[i].Ability1;
            CharData[i].Ability2 = CharDataIn[i].Ability2;
            CharData[i].RngMinPri = CharDataIn[i].RngMinPri;
            CharData[i].RngMaxPri = CharDataIn[i].RngMaxPri;
            CharData[i].RngMinSec = CharDataIn[i].RngMinSec;
            CharData[i].RngMaxSec = CharDataIn[i].RngMaxSec;
            CharData[i].CurW = CharDataIn[i].CurW;
            CharData[i].ShowSecondary = CharDataIn[i].ShowSecondary;
            CharData[i].AtkBuff = CharDataIn[i].AtkBuff;
            CharData[i].DefBuff = CharDataIn[i].DefBuff;
            CharData[i].MovBuff = CharDataIn[i].MovBuff;
            CharData[i].HpBuff = CharDataIn[i].HpBuff;
            CharData[i].Atk2 = CharDataIn[i].Atk2;
            CharData[i].Def2 = CharDataIn[i].Def2;
            CharData[i].Dead = CharDataIn[i].Dead;
            CharData[i].PosX = CharDataIn[i].PosX;
            CharData[i].PosY = CharDataIn[i].PosY;
            CharData[i].team = CharDataIn[i].team;
            CharData[i].Hp = CharDataIn[i].Hp;
            CharData[i].Atk = CharDataIn[i].Atk;
            CharData[i].Def = CharDataIn[i].Def;
            CharData[i].Mov = CharDataIn[i].Mov;
            CharData[i].PW = CharDataIn[i].PW;
            CharData[i].SW = CharDataIn[i].SW;
            CharData[i].CH = CharDataIn[i].CH;
            CharData[i].CB = CharDataIn[i].CB;
            CharData[i].name = CharDataIn[i].name;
            
            CharInitializer(CharData[i]);

            UpdateHealthBar(CharData[i]);

            //Debug.Log(new Vector3(CharDataIn[i].startRotationX, CharDataIn[i].startRotationY, CharDataIn[i].startRotationZ));

            CharData[i].CharObj.transform.RotateAround(CharData[i].CharObj.transform.position, Vector3.up, CharDataIn[i].startRotationY);
            
            //CharData[i].CharObj.transform.rotation.x = CharData[i].CharObj.transform.rotation.x;
            //CharData[i].CharObj.transform.rotation.y = CharData[i].CharObj.transform.rotation.y;
            //CharData[i].CharObj.transform.rotation.z = CharData[i].CharObj.transform.rotation.z;
            //CharData[i].CharObj.transform.rotation.w = CharData[i].CharObj.transform.rotation.w;


            /*
              load:
             
        public GameObject BodyObj = new GameObject();
        public GameObject CharObj = new GameObject();
        public GameObject HatObj = new GameObject();
        public GameObject WeaponObj = new GameObject();

        public GameObject HealthBar = new GameObject(); //2d planes for health
        public GameObject HealthBarBackLine = new GameObject();


        public Animator anim;
             
             */

        }
    }

    void LoadCharGroupIntoMap(List<localMatchIntermediateCS.CharDat> CharData, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        for(int i = 0; i < CharData.Count; i++)
        {

            TilesArray[CharData[i].PosY][CharData[i].PosX].CDat = CharData[i];

        }

    }

    public void DeleteCharArray(List<localMatchIntermediateCS.CharDat> CharData)
    {

        for (int i = 0; i < CharData.Count; i++)
        {
            if (CharData[i] != null)
            {
                if (CharData[i].CharObj != null)
                {
                    Destroy(CharData[i].CharObj);
                    Destroy(CharData[i].HatObj);
                    Destroy(CharData[i].WeaponObj);
                    Destroy(CharData[i].HealthBar);
                    Destroy(CharData[i].HealthBarBackLine);
                }
            }
        }
        CharData.Clear();
    }

    public void DeleteAllCharArraysLogic()
    {
        DeleteCharArray(localMatchIntermediateCS.LMICS.blueC);
        DeleteCharArray(localMatchIntermediateCS.LMICS.redC);
        DeleteCharArray(localMatchIntermediateCS.LMICS.yellowC);
        DeleteCharArray(localMatchIntermediateCS.LMICS.greenC);
        DeleteCharArray(localMatchIntermediateCS.LMICS.purpleC);
    }
    
    public void DeleteAllTmpTiles()
    {
        ClearTmpAtkTiles();
        ClearTmpAbilityTiles();
        ClearTmpMoveTiles();
    }

    public void DeleteMapLogic()
    {
        for (int i = 0; i < localMatchIntermediateCS.LMICS.TilesArray.GetLength(0); i++)
        {
            for (int ii = 0; ii < localMatchIntermediateCS.LMICS.TilesArray[i].GetLength(0); ii++)
            {

                //localMatchIntermediateCS.LMICS.TilesArray[i][ii] = new localMatchIntermediateCS.MapMakerVarsDat();
                //localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId = data.TilesArray[i][ii].TileId;
                Destroy(localMatchIntermediateCS.LMICS.TilesArray[i][ii].Tile); // = Instantiate(TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId].Obj, new Vector3(4 * ii, -20, 4 * i), Quaternion.identity);
                //localMatchIntermediateCS.LMICS.TilesArray[i].CDat[CharDat.y][CharDat.x] = CharDat <-- use selected char pos to also then get Cdat
            }
        }
        Array.Clear(localMatchIntermediateCS.LMICS.TilesArray, 0, localMatchIntermediateCS.LMICS.TilesArray.GetLength(0));
        


    }

    public void LoadMapStateAllLogic(string JsonString)
    {
        localMatchIntermediateCS.LocalMatchSaveMapData data = JsonConvert.DeserializeObject<localMatchIntermediateCS.LocalMatchSaveMapData>(JsonString);

        if (data != null)
        {
            //Debug.Log(JsonString);
            //localMatchIntermediateCS.LocalMatchSaveMapData data = (localMatchIntermediateCS.LocalMatchSaveMapData)bf.Deserialize(file);

            //easy vars from LMICS fill
            localMatchIntermediateCS.LMICS.SaveMapName = data.SaveMapName;
            localMatchIntermediateCS.LMICS.HighestHpWin = data.HighestHpWin;
            localMatchIntermediateCS.LMICS.HighestHpWinTurnLimit = data.HighestHpWinTurnLimit;
            localMatchIntermediateCS.LMICS.MonumentWin = data.MonumentWin;
            localMatchIntermediateCS.LMICS.MonumentTurnLimit = data.MonumentTurnLimit;
            //

            //easy vars from local match start

            LMS.Winner = data.Winner;
            LMS.CalcMoveDone = data.CalcMoveDone;
            LMS.MovePhase.MovePhase = data.MovePhase;
            LMS.CXPos = data.CXPos;
            LMS.CYPos = data.CYPos;
            LMS.currentlyFighting = data.currentlyFighting;
            LMS.currentTurn = data.currentTurn;
            LMS.CurrentTeamTurn = data.CurrentTeamTurn;
            LMS.ifLoopTurnTextFix();

Debug.Log(data.TeamCount);
            LMS.TeamCount = data.TeamCount;
            LMS.MonumentValBlue = data.MonumentValBlue;
            LMS.MonumentValRed = data.MonumentValRed;
            LMS.MonumentValYellow = data.MonumentValYellow;
            LMS.MonumentValGreen = data.MonumentValGreen;
            LMS.MonumentValPurple = data.MonumentValPurple;
            LMS.PermaHoverInfoTextOff = data.PermaHoverInfoTextOff;
            LMS.HoverInfoTMP = data.HoverInfoTMP;
            LMS.HideTileInfo = data.HideTileInfo;
            LMS.attackPrep = data.attackPrep;

            LMS.HoverTupleStore = data.HoverTupleStore; //x y pos for tuples and list
            LMS.SelectedAttackTarget = data.SelectedAttackTarget;

        //tmp tiles
        if (WebManage.WManage.MatchType == 0)
        {
            foreach (Tuple<int, int> i in data.TmpAbilityTiles)
            {
                LMS.TmpAbilityTiles[i] = Instantiate(MiscTileObj.MTO.AbilityTile, new Vector3(4 * i.Item2, -19.5f, 4 * i.Item1), Quaternion.identity); ;
            }
            foreach (Tuple<int, int> i in data.TmpMoveTiles)
            {
                LMS.TmpMoveTiles[i] = Instantiate(MiscTileObj.MTO.SelectTile, new Vector3(4 * i.Item2, -19.5f, 4 * i.Item1), Quaternion.identity); ;
            }
            foreach (Tuple<int, int> i in data.TmpAtkTiles)
            {
                LMS.TmpAtkTiles[i] = Instantiate(MiscTileObj.MTO.AttackTile, new Vector3(4 * i.Item2, -19.5f, 4 * i.Item1), Quaternion.identity); ;
            }
        }

            //load map
            localMatchIntermediateCS.LMICS.TilesArray = new localMatchIntermediateCS.MapMakerVarsDat[data.TilesArray.GetLength(0)][];
            for (int i = 0; i < data.TilesArray.GetLength(0); i++)
            {
                localMatchIntermediateCS.LMICS.TilesArray[i] = new localMatchIntermediateCS.MapMakerVarsDat[data.TilesArray[0].GetLength(0)];
                for (int ii = 0; ii < data.TilesArray[0].GetLength(0); ii++)
                {

                    localMatchIntermediateCS.LMICS.TilesArray[i][ii] = new localMatchIntermediateCS.MapMakerVarsDat();
                    localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId = data.TilesArray[i][ii].TileId;
                    localMatchIntermediateCS.LMICS.TilesArray[i][ii].Tile = Instantiate(TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId].Obj, new Vector3(4 * ii, -20, 4 * i), Quaternion.identity);
                    //localMatchIntermediateCS.LMICS.TilesArray[i].CDat[CharDat.y][CharDat.x] = CharDat <-- use selected char pos to also then get Cdat
                }
            }

        //load chars with items and material into group and dictionary
        try
        {
            LoadCharsInto(localMatchIntermediateCS.LMICS.blueC, data.BlueChar);
            LoadCharsInto(localMatchIntermediateCS.LMICS.redC, data.RedChar);
            LoadCharsInto(localMatchIntermediateCS.LMICS.yellowC, data.YellowChar);
            LoadCharsInto(localMatchIntermediateCS.LMICS.greenC, data.GreenChar);
            LoadCharsInto(localMatchIntermediateCS.LMICS.purpleC, data.PurpleChar);
        }
        catch { Debug.Log("Failed to Load CharData into arrays - map:\n"+ JsonString); }
        try { 
            LoadCharGroupIntoMap(localMatchIntermediateCS.LMICS.blueC, localMatchIntermediateCS.LMICS.TilesArray);
            LoadCharGroupIntoMap(localMatchIntermediateCS.LMICS.redC, localMatchIntermediateCS.LMICS.TilesArray);
            LoadCharGroupIntoMap(localMatchIntermediateCS.LMICS.yellowC, localMatchIntermediateCS.LMICS.TilesArray);
            LoadCharGroupIntoMap(localMatchIntermediateCS.LMICS.greenC, localMatchIntermediateCS.LMICS.TilesArray);
            LoadCharGroupIntoMap(localMatchIntermediateCS.LMICS.purpleC, localMatchIntermediateCS.LMICS.TilesArray);
        }
        catch { Debug.Log("Failed to Load CharData into map - map:\n"+ JsonString); }

            try
            {
                if (data.SelectedCharX != -1)
                {

                    LMS.SelectedChar = localMatchIntermediateCS.LMICS.TilesArray[data.SelectedCharY][data.SelectedCharX].CDat;

                    Renderer[] ComponentR; //color selected char
                    ComponentR = LMS.SelectedChar.CharObj.GetComponentsInChildren<Renderer>();
                    for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                    {

                        ComponentR[tmp].material = ConstantCharObject.CCObj.Selected;

                    }
                }
            }
            catch { Debug.Log("Failed to Load selected Char"); }

            LMS.TurnCountText.text = "" + (((LMS.currentTurn - 1) / LMS.TeamCount)); //set turn count visual
        }

        if(LMS.TeamCount == 0 || LMS.TeamCount == null){
            Debug.Log("teamCount works?");

            LMS.TeamCount = 0;
            if(localMatchIntermediateCS.LMICS.blueC.Count != 0) { LMS.TeamCount += 1; }
            if (localMatchIntermediateCS.LMICS.redC.Count != 0) {  LMS.TeamCount += 1; }
            if (localMatchIntermediateCS.LMICS.yellowC.Count != 0) {  LMS.TeamCount += 1; }
            if (localMatchIntermediateCS.LMICS.greenC.Count != 0) {  LMS.TeamCount += 1; }
            if (localMatchIntermediateCS.LMICS.purpleC.Count != 0) {  LMS.TeamCount += 1; }
            
            Debug.Log("teamCount is:"+ LMS.TeamCount.ToString());
        }
        
                    //Window State vars
            if (WebManage.WManage.MatchType == 0)
            {
                LMS.HoverInfo.SetActive(data.IsHoverInfoActive);
                LMS.HoverInfoCloseButton.SetActive(data.IsHoverInfoCloseButtonActive);
                LMS.ActionMenuObj.SetActive(data.IsActionMenuObjActive);
                LMS.CloseActionMenuObj.SetActive(data.IsCloseActionMenuObjActive);
                LMS.CloseActionAtkObj.SetActive(data.IsCloseActionAtkObjActive);
                LMS.ActionAbilityInfoObj.SetActive(data.IsActionAbilityInfoObjActive);
                LMS.CloseActionAbilityObj.SetActive(data.IsCloseActionAbilityObjActive);
                LMS.Attack1.SetActive(data.IsAttack1Active);
                LMS.Attack2.SetActive(data.AIsttack2Active);
                LMS.Ability1.SetActive(data.IsAbility1Active);
                LMS.Ability2.SetActive(data.IsAbility2Active);
                LMS.Move.SetActive(data.IsMoveActive);
                LMS.TileInfoObj.SetActive(data.IsTileInfoObjActive);
                LMS.TileInfoToggle.SetActive(data.IsTileInfoToggleActive);
                LMS.AttackPrepScreenAttacker.SetActive(data.IsAttackPrepScreenAttackerActive);
                LMS.AttackPrepScreenDefender.SetActive(data.IsAttackPrepScreenDefenderActive);
                LMS.StartToAttackButton.SetActive(data.IsStartToAttackButtonActive);
                LMS.CloseActionRotateObjBackButton.SetActive(data.IsCloseActionRotateObjBackButtonActive);
                LMS.WinConditionObj.SetActive(data.IsWinConditionObjActive);
            }
            else if(WebManage.WManage.MatchType == 1 && TeamOrderSameAsCurrentTurn() == false)
            {
                    //LMS.HoverInfo.SetActive(false);
                    //LMS.HoverInfoCloseButton.SetActive(data.IsHoverInfoCloseButtonActive);
                    LMS.ActionMenuObj.SetActive(false);
                    LMS.CloseActionMenuObj.SetActive(false);
                    LMS.CloseActionAtkObj.SetActive(false);
                    LMS.ActionAbilityInfoObj.SetActive(false);
                    LMS.CloseActionAbilityObj.SetActive(false);
                    LMS.Attack1.SetActive(false);
                    LMS.Attack2.SetActive(false);
                    LMS.Ability1.SetActive(false);
                    LMS.Ability2.SetActive(false);
                    LMS.Move.SetActive(false);
                    //LMS.TileInfoObj.SetActive();
                    //LMS.TileInfoToggle.SetActive(data.IsTileInfoToggleActive);
                    LMS.AttackPrepScreenAttacker.SetActive(false);
                    LMS.AttackPrepScreenDefender.SetActive(false);
                    LMS.StartToAttackButton.SetActive(false);
                    LMS.CloseActionRotateObjBackButton.SetActive(false);
                    //LMS.WinConditionObj.SetActive(data.IsWinConditionObjActive);
                
            }

    }

    public void LoadMapStateAll() //todo: load match var has a problem with menu's if already open or smthing and closing the OG action menu - only problem that exists
    {
        LMS.LoadMapStateAllLogic(File.ReadAllText(StaticDataMapMaker.controlObj.LoadMapDatPath));
    }

    public void LoadMapStateBat1() //todo: load match var has a problem with menu's if already open or smthing and closing the OG action menu - only problem that exists
    {
        LMS.LoadMapStateAllLogic(WebManage.WManage.JsonReceiveS.s);
    }

    public bool CheckToLoadMapFromWeb()
    {
        if (WebManage.WManage.GivenMapData)
        {
            return true;// not retunr given map dat incase I need extra logic...
        }
        else
        {
            return false;
        }
    }

    public void CheckAndLoadWebMapBat1()
    {
        if (CheckToLoadMapFromWeb())
        {
            WebManage.WManage.GivenMapData = false;
            LoadMapStateBat1();
        }
    }

    public void CheckAndLoadWebMapALLBat1()
    {
        if (CheckToLoadMapFromWeb())
        {
            WebManage.WManage.GivenMapData = false;
            LMS.DeleteMapLogic();
            LMS.DeleteAllCharArraysLogic();
            LMS.DeleteAllTmpTiles();

            LMS.LoadMapStateBat1();
        }
    }

    public void ToggleWinConditionView()
    {

        LMS.WinConditionObj.SetActive(!LMS.WinConditionObj.activeSelf);

        LMS.WinCondition.text = "- Kill all of the other team";

        if (localMatchIntermediateCS.LMICS.HighestHpWin)
        {

            int BlueHp = 0;
            int RedHp = 0;
            int YellowHp = 0;
            int GreenHp = 0;
            int PurpleHp = 0;

            for (int i = 0; i < localMatchIntermediateCS.LMICS.blueC.Count; i++)
            {
                BlueHp += localMatchIntermediateCS.LMICS.blueC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.redC.Count; i++)
            {
                RedHp += localMatchIntermediateCS.LMICS.redC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.yellowC.Count; i++)
            {
                YellowHp += localMatchIntermediateCS.LMICS.yellowC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.greenC.Count; i++)
            {
                GreenHp += localMatchIntermediateCS.LMICS.greenC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.purpleC.Count; i++)
            {
                PurpleHp += localMatchIntermediateCS.LMICS.purpleC[i].Hp;
            }

            LMS.WinCondition.text += "\n\nOR\n\n- Have the highest net Hp team by the turn of: " + localMatchIntermediateCS.LMICS.HighestHpWinTurnLimit;
            LMS.WinCondition.text += "\nCurrent Blue Hp: " + BlueHp;
            LMS.WinCondition.text += "\nCurrent Red Hp: " + RedHp;
            LMS.WinCondition.text += "\nCurrent Green Hp: " + YellowHp;
            LMS.WinCondition.text += "\nCurrent Yellow Hp: " + GreenHp;
            LMS.WinCondition.text += "\nCurrent Purple Hp: " + PurpleHp;
        }
        if (localMatchIntermediateCS.LMICS.MonumentWin)
        {
            LMS.WinCondition.text += "\n\nOR\n\nStand on a monument for a net turn count [not dead] of: " + localMatchIntermediateCS.LMICS.MonumentTurnLimit;
            //print current numbers
            LMS.WinCondition.text += "\nCurrent Blue Turn-Count: " + MonumentValBlue;
            LMS.WinCondition.text += "\nCurrent Red Turn-Count: " + MonumentValRed;
            LMS.WinCondition.text += "\nCurrent Green Turn-Count: " + MonumentValYellow;
            LMS.WinCondition.text += "\nCurrent Yellow Turn-Count: " + MonumentValGreen;
            LMS.WinCondition.text += "\nCurrent Purple Turn-Count: " + MonumentValPurple;

        }

    }

    public void RotateCharBackButtonClick()
    {
        LMS.MovePhase.MovePhase = 0;

        LMS.ActionMenuObj.SetActive(true);
        LMS.CloseActionMenuObj.SetActive(true);
        LMS.CloseActionRotateObjBackButton.SetActive(false);
        ArrowLineDrawer.Destroy();
    }

    public void rotateCharAction()
    {
        LMS.MovePhase.MovePhase = 3;

        LMS.ActionMenuObj.SetActive(false);
        LMS.CloseActionMenuObj.SetActive(false);
        LMS.CloseActionRotateObjBackButton.SetActive(true);

    }

    void DrawRayDirection(int x, int y, int x2, int y2)
    {
        ArrowLineDrawer.DrawLineInGameView(new Vector3(x, -17, y), new Vector3(x2, -17, y2), Color.yellow);
    }

    void rotateCharTheRotation(int x, int y)
    {//-19
        //Debug.Log((SelectedChar.CharObj.transform.position - new Vector3(x * 4, SelectedChar.CharObj.transform.position.y, y * 4)));

        SelectedChar.CharObj.transform.rotation = Quaternion.LookRotation( (SelectedChar.CharObj.transform.position - new Vector3(x*4, SelectedChar.CharObj.transform.position.y, y*4) ) ); //may need to use 1 degree or diffrent intervals or raise
        
        SelectedChar.CharObj.transform.RotateAround(SelectedChar.CharObj.transform.position, Vector3.up, -90); //account for -90

        LMS.SelectedChar.HasTurn = false;
        LMS.SelectedChar.MovedAlready = true;

        LMS.CloseActionMenuAndDeselect();

        LMS.SelectedChar = null;

    }

    void ClearTmpAtkTiles()
    {
        foreach (Tuple<int, int> i in LMS.TmpAtkTiles.Keys)
        {

            Destroy(LMS.TmpAtkTiles[i]);

        }
        LMS.TmpAtkTiles.Clear();
    }

    void LoadCharInfoLocalMatchStart(List<localMatchIntermediateCS.CharDat> CharData, List<localMatchIntermediateCS.InFlightCharData> CharDataIn)
    {
        for (int i = 0; i < CharData.Count; i++)
        {

            Debug.Log("Char Passed Load Into Match Start: "+i.ToString());

            localMatchIntermediateCS.InFlightCharData tmp = new localMatchIntermediateCS.InFlightCharData();

            CharDataIn.Add(tmp);

            CharDataIn[i].HasTurn = CharData[i].HasTurn;
            CharDataIn[i].MovedAlready = CharData[i].MovedAlready;
            CharDataIn[i].Ability1 = CharData[i].Ability1;
            CharDataIn[i].Ability2 = CharData[i].Ability2;
            CharDataIn[i].RngMinPri = CharData[i].RngMinPri;
            CharDataIn[i].RngMaxPri = CharData[i].RngMaxPri;
            CharDataIn[i].RngMinSec = CharData[i].RngMinSec;
            CharDataIn[i].RngMaxSec = CharData[i].RngMaxSec;
            CharDataIn[i].CurW = CharData[i].CurW;
            CharDataIn[i].ShowSecondary = CharData[i].ShowSecondary;
            CharDataIn[i].AtkBuff = CharData[i].AtkBuff;
            CharDataIn[i].DefBuff = CharData[i].DefBuff;
            CharDataIn[i].MovBuff = CharData[i].MovBuff;
            CharDataIn[i].HpBuff = CharData[i].HpBuff;
            CharDataIn[i].Atk2 = CharData[i].Atk2;
            CharDataIn[i].Def2 = CharData[i].Def2;
            CharDataIn[i].Dead = CharData[i].Dead;
            CharDataIn[i].PosX = CharData[i].PosX;
            CharDataIn[i].PosY = CharData[i].PosY;
            CharDataIn[i].team = CharData[i].team;
            CharDataIn[i].Hp = CharData[i].Hp;
            CharDataIn[i].Atk = CharData[i].Atk;
            CharDataIn[i].Def = CharData[i].Def;
            CharDataIn[i].Mov = CharData[i].Mov;
            CharDataIn[i].PW = CharData[i].PW;
            CharDataIn[i].SW = CharData[i].SW;
            CharDataIn[i].CH = CharData[i].CH;
            CharDataIn[i].CB = CharData[i].CB;
            CharDataIn[i].name = CharData[i].name;

            CharDataIn[i].startRotationX = CharData[i].CharObj.transform.rotation.eulerAngles.x;
            CharDataIn[i].startRotationY = CharData[i].CharObj.transform.rotation.eulerAngles.y;
            CharDataIn[i].startRotationZ = CharData[i].CharObj.transform.rotation.eulerAngles.z;
            
        }
    }

    void AtkDFSNextTile(int x, int y, int depth, int MinRange, int startX, int startY)
    {
        if (TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[y][x].TileId].MoveCost == 100) //wall - muh boi, da wall is in the way!
        {

            //stops

        }
        else
        {
            if (depth >= 0 && (Math.Abs(startX - x) + Math.Abs(startY - y)) >= MinRange)  //depth - 1 <= MinRange && 
            {


                if (!TmpAtkTiles.ContainsKey(Tuple.Create(y, x)))
                {
                    TmpAtkTiles[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.AttackTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }
                AtkDFS(x, y, depth - 1, MinRange, startX, startY);  //minus tile ID mov cost


            }
            else if (depth - 1 >= 0)
            {

                AtkDFS(x, y, depth - 1, MinRange, startX, startY);  //minus tile ID mov cost

            }
        }

    }

    void AtkDFS(int x, int y, int depth, int MinRange, int startX, int startY) //y is initial y pos - x is initial x pos - depth is initial char movment
    {

        if (depth > 0)
        {
            if (x > 0)
            {
                AtkDFSNextTile(x - 1, y, depth, MinRange, startX, startY);
            }
            if (x < localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0) - 1)
            {
                AtkDFSNextTile(x + 1, y, depth, MinRange, startX, startY);
            }
            if (y > 0)
            {
                AtkDFSNextTile(x, y - 1, depth, MinRange, startX, startY);
            }
            if (y < localMatchIntermediateCS.LMICS.TilesArray.GetLength(0) - 1)
            {
                AtkDFSNextTile(x, y + 1, depth, MinRange, startX, startY);
            }
        }

    }

    void TrueAttackPhaseLogic()
    {
        LMS.MovePhase.MovePhase = 2;

        LMS.ActionMenuObj.SetActive(false);
        LMS.CloseActionMenuObj.SetActive(false);
        LMS.CloseActionAtkObj.SetActive(true);

        if (LMS.SelectedChar.CurW == 1)
        {

            AtkDFS(LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, LMS.SelectedChar.RngMaxPri, LMS.SelectedChar.RngMinPri, LMS.SelectedChar.PosX, LMS.SelectedChar.PosY);

        }

        else if (LMS.SelectedChar.CurW == 2)
        {

            AtkDFS(LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, LMS.SelectedChar.RngMaxSec, LMS.SelectedChar.RngMinSec, LMS.SelectedChar.PosX, LMS.SelectedChar.PosY);

        }


    }

    public void StartAttackPhasePrimary()
    {

        LMS.SelectedChar.CurW = 1;
        Destroy(LMS.SelectedChar.WeaponObj);
        LMS.SelectedChar.WeaponObj = Instantiate(PriWLookup.PWLO.PriWeapons[LMS.SelectedChar.PW], new Vector3(0, 0, 0), Quaternion.identity);

        ParentConstraint pc3 = LMS.SelectedChar.WeaponObj.AddComponent<ParentConstraint>();
        ConstraintSource constraintSource2 = new ConstraintSource();
        constraintSource2.sourceTransform = LMS.SelectedChar.CharObj.transform.Find(ConstantCharObject.CCObj.WHandString);
        constraintSource2.weight = 1;
        pc3.AddSource(constraintSource2);
        pc3.constraintActive = true;
        //        pc1.rotationAxis = Axis.None;
        pc3.SetTranslationOffset(0, new Vector3(-1.0f, 0.0f, 0.0f));
        pc3.SetRotationOffset(0, new Vector3(270.0f, 90.0f, 0.0f));


        LMS.TrueAttackPhaseLogic();

    }

    public void StartAttackPhaseSecondary()
    {
        LMS.SelectedChar.CurW = 2; // attack using second weapon - you now lose all buff
        Destroy(LMS.SelectedChar.WeaponObj);
        LMS.SelectedChar.WeaponObj = Instantiate(SecWLookup.SWLO.SecWeapons[LMS.SelectedChar.SW], new Vector3(0, 0, 0), Quaternion.identity);

        ParentConstraint pc3 = LMS.SelectedChar.WeaponObj.AddComponent<ParentConstraint>();
        ConstraintSource constraintSource2 = new ConstraintSource();
        constraintSource2.sourceTransform = LMS.SelectedChar.CharObj.transform.Find(ConstantCharObject.CCObj.WHandString);
        constraintSource2.weight = 1;
        pc3.AddSource(constraintSource2);
        pc3.constraintActive = true;
        //        pc1.rotationAxis = Axis.None;
        pc3.SetTranslationOffset(0, new Vector3(-1.0f, 0.0f, 0.0f));
        pc3.SetRotationOffset(0, new Vector3(270.0f, 90.0f, 0.0f));



        LMS.TrueAttackPhaseLogic();
    }


    void TrueAbilityPhaseLogic()
    {
        
        LMS.MovePhase.MovePhase = 4;

        LMS.ActionMenuObj.SetActive(false);
        LMS.CloseActionMenuObj.SetActive(false);
        LMS.CloseActionAbilityObj.SetActive(true);
        LMS.ActionAbilityInfoObj.SetActive(true);

        if (LMS.SelectedChar.CurW == 1)
        {
            ActionAbilityInfoText.text = AbilityLookUpTable.ALUT.AbilityName[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW]] + "\n\n"+AbilityLookUpTable.ALUT.AbilityBlurb[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW]];
            //Debug.Log(AbilityLookUpTable.ALUT.RangeFuncType[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW]]);
            AbilityLookUpTable.ALUT.RangeFuncCalc[ AbilityLookUpTable.ALUT.RangeFuncType[ PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW] ] ](LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, AbilityLookUpTable.ALUT.RangeMax[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW]], AbilityLookUpTable.ALUT.RangeMin[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW]], LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, localMatchIntermediateCS.LMICS.TilesArray, TmpAbilityTiles, LMS.SelectedChar);

        }

        else if (LMS.SelectedChar.CurW == 2)
        {
            ActionAbilityInfoText.text = AbilityLookUpTable.ALUT.AbilityName[SecWLookup.SWLO.AbilityNumber[LMS.SelectedChar.SW]] + "\n\n" + AbilityLookUpTable.ALUT.AbilityBlurb[SecWLookup.SWLO.AbilityNumber[LMS.SelectedChar.SW]];

            AbilityLookUpTable.ALUT.RangeFuncCalc[ AbilityLookUpTable.ALUT.RangeFuncType[ SecWLookup.SWLO.AbilityNumber[LMS.SelectedChar.SW] ] ](LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, AbilityLookUpTable.ALUT.RangeMax[SecWLookup.SWLO.AbilityNumber[LMS.SelectedChar.SW]], AbilityLookUpTable.ALUT.RangeMin[SecWLookup.SWLO.AbilityNumber[LMS.SelectedChar.SW]], LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, localMatchIntermediateCS.LMICS.TilesArray, TmpAbilityTiles, LMS.SelectedChar);

        }


    }

    public void StartAbilityPhasePrimary()
    {
        if (LMS.SelectedChar.Ability1 == 0)
        {
            ClearTmpAtkTiles();

            LMS.SelectedChar.CurW = 1;
            Destroy(LMS.SelectedChar.WeaponObj);
            LMS.SelectedChar.WeaponObj = Instantiate(PriWLookup.PWLO.PriWeapons[LMS.SelectedChar.PW], new Vector3(0, 0, 0), Quaternion.identity);

            ParentConstraint pc3 = LMS.SelectedChar.WeaponObj.AddComponent<ParentConstraint>();
            ConstraintSource constraintSource2 = new ConstraintSource();
            constraintSource2.sourceTransform = LMS.SelectedChar.CharObj.transform.Find(ConstantCharObject.CCObj.WHandString);
            constraintSource2.weight = 1;
            pc3.AddSource(constraintSource2);
            pc3.constraintActive = true;
            //        pc1.rotationAxis = Axis.None;
            pc3.SetTranslationOffset(0, new Vector3(-1.0f, 0.0f, 0.0f));
            pc3.SetRotationOffset(0, new Vector3(270.0f, 90.0f, 0.0f));


            LMS.TrueAbilityPhaseLogic();
        }
    }

    public void StartAbilityPhaseSecondary()
    {
        if (LMS.SelectedChar.Ability2 == 0)
        {
            ClearTmpAtkTiles();

            LMS.SelectedChar.CurW = 2; // attack using second weapon - you now lose all buff
            Destroy(LMS.SelectedChar.WeaponObj);
            LMS.SelectedChar.WeaponObj = Instantiate(SecWLookup.SWLO.SecWeapons[LMS.SelectedChar.SW], new Vector3(0, 0, 0), Quaternion.identity);

            ParentConstraint pc3 = LMS.SelectedChar.WeaponObj.AddComponent<ParentConstraint>();
            ConstraintSource constraintSource2 = new ConstraintSource();
            constraintSource2.sourceTransform = LMS.SelectedChar.CharObj.transform.Find(ConstantCharObject.CCObj.WHandString);
            constraintSource2.weight = 1;
            pc3.AddSource(constraintSource2);
            pc3.constraintActive = true;
            //        pc1.rotationAxis = Axis.None;
            pc3.SetTranslationOffset(0, new Vector3(-1.0f, 0.0f, 0.0f));
            pc3.SetRotationOffset(0, new Vector3(270.0f, 90.0f, 0.0f));



            LMS.TrueAbilityPhaseLogic();
        }
    }

    void AbilityClickLogic(int x, int y)
    {

        if (LMS.TmpAbilityTiles.ContainsKey(Tuple.Create(y, x))) //&& localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat != null && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.Dead == false && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.team != LMS.SelectedChar.team <-- this will be apart of the ability - some abilities may change terrain
        {
            if (LMS.SelectedChar.CurW == 1)
            {
                AbilityLookUpTable.ALUT.AttackFunc[AbilityLookUpTable.ALUT.AffectFuncType[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.PW]]](localMatchIntermediateCS.LMICS.TilesArray, LMS.SelectedChar, y, x, LMS.MovePhase, LMS.CalcMoveDone, LMS.ActionAbilityInfoObj, LMS.CloseActionAbilityObj, LMS.TmpAbilityTiles);
            }
            else if (LMS.SelectedChar.CurW == 2)
            {
                AbilityLookUpTable.ALUT.AttackFunc[AbilityLookUpTable.ALUT.AffectFuncType[PriWLookup.PWLO.AbilityNumber[LMS.SelectedChar.SW]]](localMatchIntermediateCS.LMICS.TilesArray, LMS.SelectedChar, y, x, LMS.MovePhase, LMS.CalcMoveDone, LMS.ActionAbilityInfoObj, LMS.CloseActionAbilityObj, LMS.TmpAbilityTiles);
            }

        }

    }

    void WinLogic()
    {
        bool allBlueDead = true;
        bool allRedDead = true;
        bool allYellowDead = true;
        bool allGreenDead = true;
        bool allPurpleDead = true;


        for (int i = 0; i < localMatchIntermediateCS.LMICS.blueC.Count; i++)
        {
            if(localMatchIntermediateCS.LMICS.blueC[i].Dead == false)
            {
                allBlueDead = false;
                i = localMatchIntermediateCS.LMICS.blueC.Count;
            }
        }

        for (int i = 0; i < localMatchIntermediateCS.LMICS.redC.Count; i++)
        {
            if(localMatchIntermediateCS.LMICS.redC[i].Dead == false)
            {
                allRedDead = false;
                i = localMatchIntermediateCS.LMICS.redC.Count;
            }
        }


        for (int i = 0; i < localMatchIntermediateCS.LMICS.yellowC.Count; i++)
        {
            if (localMatchIntermediateCS.LMICS.yellowC[i].Dead == false)
            {
                allYellowDead = false;
                i = localMatchIntermediateCS.LMICS.yellowC.Count;

            }
        }


        for (int i = 0; i < localMatchIntermediateCS.LMICS.greenC.Count; i++)
        {
            if (localMatchIntermediateCS.LMICS.greenC[i].Dead == false)
            {
                allGreenDead = false;
                i = localMatchIntermediateCS.LMICS.greenC.Count;
            }
        }


        for (int i = 0; i < localMatchIntermediateCS.LMICS.purpleC.Count; i++)
        {
            if (localMatchIntermediateCS.LMICS.purpleC[i].Dead == false)
            {
                allPurpleDead = false;
                i = localMatchIntermediateCS.LMICS.purpleC.Count;
            }
        }

        //        if(allBlueDead == true && allRedDead == true && allYellowDead == true && allGreenDead == true && allPurpleDead == true)
        if (allBlueDead == false && allRedDead == true && allYellowDead == true && allGreenDead == true && allPurpleDead == true)
        {
            LMS.Winner = 1;
        }
        else if(allBlueDead == true && allRedDead == false && allYellowDead == true && allGreenDead == true && allPurpleDead == true)
        {
            LMS.Winner = 2;
        }
        else if (allBlueDead == true && allRedDead == true && allYellowDead == false && allGreenDead == true && allPurpleDead == true)
        {
            LMS.Winner = 3;
        }
        else if (allBlueDead == true && allRedDead == true && allYellowDead == true && allGreenDead == false && allPurpleDead == true)
        {
            LMS.Winner = 4;
        }
        else if (allBlueDead == true && allRedDead == true && allYellowDead == true && allGreenDead == true && allPurpleDead == false)
        {
            LMS.Winner = 5;
        }
        else if (localMatchIntermediateCS.LMICS.HighestHpWin == true && (localMatchIntermediateCS.LMICS.HighestHpWinTurnLimit * LMS.TeamCount)+1 == LMS.currentTurn ) //if highest HP win condition flag - by a certain turn - the turn is current/4 , and the turn shown to the player is current/4 since I do 1 turn for every 1/4th of a full cycle - bias towards blue player
        { //get highest HP, bias towards Blue for winner when desiered turn count is reached
            int BlueHp = 0;
            int RedHp = 0;
            int YellowHp = 0;
            int GreenHp = 0;
            int PurpleHp = 0;

            for (int i = 0; i < localMatchIntermediateCS.LMICS.blueC.Count; i++)
            {
                BlueHp += localMatchIntermediateCS.LMICS.blueC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.redC.Count; i++)
            {
                RedHp += localMatchIntermediateCS.LMICS.redC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.yellowC.Count; i++)
            {
                YellowHp += localMatchIntermediateCS.LMICS.yellowC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.greenC.Count; i++)
            {
                GreenHp += localMatchIntermediateCS.LMICS.greenC[i].Hp;
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.purpleC.Count; i++)
            {
                PurpleHp += localMatchIntermediateCS.LMICS.purpleC[i].Hp;
            }

            List<int> ListHP = new List<int>{ BlueHp, RedHp, YellowHp, GreenHp, PurpleHp };
            int maxIndex = 0;
            int maxVal = 0;

            for(int i = 0; i < ListHP.Count; i++)
            {
                if(maxVal < ListHP[i])
                {
                    maxVal = ListHP[i];
                    maxIndex = i+1;
                }
            }

            switch (maxIndex)
            {
                case 1:
                    LMS.Winner = 1;
                    break;
                case 2:
                    LMS.Winner = 2;
                    break;
                case 3:
                    LMS.Winner = 3;
                    break;
                case 4:
                    LMS.Winner = 4;
                    break;
                case 5:
                    LMS.Winner = 5;
                    break;
            }

        }
        else if (localMatchIntermediateCS.LMICS.MonumentWin == true && (MonumentValBlue >= localMatchIntermediateCS.LMICS.MonumentTurnLimit || MonumentValRed >= localMatchIntermediateCS.LMICS.MonumentTurnLimit || MonumentValYellow >= localMatchIntermediateCS.LMICS.MonumentTurnLimit || MonumentValGreen >= localMatchIntermediateCS.LMICS.MonumentTurnLimit || MonumentValPurple >= localMatchIntermediateCS.LMICS.MonumentTurnLimit) ) //have flag for if this mode of cap point is on - check in order if anybody reaches the "cap the point" turn limit then you wim - use || to make sure it hits if true and then gets result - bias towards blue... 
        {
            //get higher monument at moment in case funny stuff happens

            List<int> ListMonVal = new List<int> { MonumentValBlue, MonumentValRed, MonumentValYellow, MonumentValGreen, MonumentValPurple};
            int maxIndex = 0;
            int maxVal = 0;

            for (int i = 0; i < ListMonVal.Count; i++)
            {
                if (maxVal < ListMonVal[i])
                {
                    maxVal = ListMonVal[i];
                    maxIndex = i + 1;
                }
            }

            switch (maxIndex)
            {
                case 1:
                    LMS.Winner = 1;
                    break;
                case 2:
                    LMS.Winner = 2;
                    break;
                case 3:
                    LMS.Winner = 3;
                    break;
                case 4:
                    LMS.Winner = 4;
                    break;
                case 5:
                    LMS.Winner = 5;
                    break;
            }

        }


        switch (Winner)
        {
            case 1:
                Debug.Log("Blue Wins");
                break;
            case 2:
                Debug.Log("Red Wins");
                break;
            case 3:
                Debug.Log("Green Wins");
                break;
            case 4:
                Debug.Log("Yellow Wins");
                break;
            case 5:
                Debug.Log("Purple Wins");
                break;
        }
    }

    bool CompareMonument(localMatchIntermediateCS.CharDat Cdat)
    {//redundat to check if dead again... still do it for *reasons*

        if(Cdat.Dead == false && (localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId == 51 || localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId == 52 || localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId == 53))
        {

            return true;

        }

        return false;

    }
    
    void NewTurnLogicBat0()
    {
        if (LMS.currentlyFighting == false) //only work once fighting anim is done
        {

            CloseActionMenuAndDeselect();

            Renderer[] ComponentR;
            if (LMS.CurrentTeamTurn == 1)
            {
                for (int i = 0; i < localMatchIntermediateCS.LMICS.blueC.Count; i++)
                {

                    localMatchIntermediateCS.LMICS.blueC[i].MovedAlready = false;

                    ComponentR = localMatchIntermediateCS.LMICS.blueC[i].CharObj.GetComponentsInChildren<Renderer>();
                    for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                    {
                        ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[localMatchIntermediateCS.LMICS.blueC[i].team - 1];
                    }

                    if (localMatchIntermediateCS.LMICS.blueC[i].Dead == false)
                    {
                        HpBuffHeal(localMatchIntermediateCS.LMICS.blueC[i]);

                        AdjustStatsAll(localMatchIntermediateCS.LMICS.blueC[i]);

                        EndTileStatAdjustAll(localMatchIntermediateCS.LMICS.blueC[i]);

                        NewTurnDeathHandle(localMatchIntermediateCS.LMICS.blueC[i]);

                        if (CompareMonument(localMatchIntermediateCS.LMICS.blueC[i])) //die and give points is possible... :smile:
                        {

                            MonumentValBlue += 1;

                        }
                    }

                }
            }
            else if (LMS.CurrentTeamTurn == 2)
            {
                for (int i = 0; i < localMatchIntermediateCS.LMICS.redC.Count; i++)
                {
                    localMatchIntermediateCS.LMICS.redC[i].MovedAlready = false;
                    ComponentR = localMatchIntermediateCS.LMICS.redC[i].CharObj.GetComponentsInChildren<Renderer>();
                    for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                    {
                        ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[localMatchIntermediateCS.LMICS.redC[i].team - 1];
                    }

                    if (localMatchIntermediateCS.LMICS.redC[i].Dead == false)
                    {
                        HpBuffHeal(localMatchIntermediateCS.LMICS.redC[i]);

                        AdjustStatsAll(localMatchIntermediateCS.LMICS.redC[i]);

                        EndTileStatAdjustAll(localMatchIntermediateCS.LMICS.redC[i]);

                        NewTurnDeathHandle(localMatchIntermediateCS.LMICS.redC[i]);

                        if (CompareMonument(localMatchIntermediateCS.LMICS.redC[i]))
                        {

                            MonumentValRed += 1;

                        }
                    }
                }
            }
            else if (LMS.CurrentTeamTurn == 3)
            {
                for (int i = 0; i < localMatchIntermediateCS.LMICS.yellowC.Count; i++)
                {
                    localMatchIntermediateCS.LMICS.yellowC[i].MovedAlready = false;
                    ComponentR = localMatchIntermediateCS.LMICS.yellowC[i].CharObj.GetComponentsInChildren<Renderer>();
                    for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                    {
                        ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[localMatchIntermediateCS.LMICS.yellowC[i].team - 1];
                    }
                    if (localMatchIntermediateCS.LMICS.yellowC[i].Dead == false)
                    {
                        HpBuffHeal(localMatchIntermediateCS.LMICS.yellowC[i]);

                        AdjustStatsAll(localMatchIntermediateCS.LMICS.yellowC[i]);

                        EndTileStatAdjustAll(localMatchIntermediateCS.LMICS.yellowC[i]);

                        NewTurnDeathHandle(localMatchIntermediateCS.LMICS.yellowC[i]);

                        if (CompareMonument(localMatchIntermediateCS.LMICS.yellowC[i]))
                        {

                            MonumentValYellow += 1;

                        }
                    }
                }
            }
            else if (LMS.CurrentTeamTurn == 4)
            {
                for (int i = 0; i < localMatchIntermediateCS.LMICS.greenC.Count; i++)
                {
                    localMatchIntermediateCS.LMICS.greenC[i].MovedAlready = false;
                    ComponentR = localMatchIntermediateCS.LMICS.greenC[i].CharObj.GetComponentsInChildren<Renderer>();
                    for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                    {
                        ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[localMatchIntermediateCS.LMICS.greenC[i].team - 1];
                    }

                    if (localMatchIntermediateCS.LMICS.greenC[i].Dead == false)
                    {
                        HpBuffHeal(localMatchIntermediateCS.LMICS.greenC[i]);

                        AdjustStatsAll(localMatchIntermediateCS.LMICS.greenC[i]);

                        EndTileStatAdjustAll(localMatchIntermediateCS.LMICS.greenC[i]);

                        NewTurnDeathHandle(localMatchIntermediateCS.LMICS.greenC[i]);

                        if (CompareMonument(localMatchIntermediateCS.LMICS.greenC[i]))
                        {

                            MonumentValGreen += 1;

                        }
                    }
                }
            }
            else if (LMS.CurrentTeamTurn == 5)
            {
                for (int i = 0; i < localMatchIntermediateCS.LMICS.purpleC.Count; i++)
                {
                    localMatchIntermediateCS.LMICS.purpleC[i].MovedAlready = false; // I should link purpleC to an array and just index like teams...
                    ComponentR = localMatchIntermediateCS.LMICS.purpleC[i].CharObj.GetComponentsInChildren<Renderer>();
                    for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                    {
                        ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[localMatchIntermediateCS.LMICS.purpleC[i].team - 1];
                    }

                    if (localMatchIntermediateCS.LMICS.purpleC[i].Dead == false)
                    {
                        HpBuffHeal(localMatchIntermediateCS.LMICS.purpleC[i]);

                        AdjustStatsAll(localMatchIntermediateCS.LMICS.purpleC[i]);

                        EndTileStatAdjustAll(localMatchIntermediateCS.LMICS.purpleC[i]);

                        NewTurnDeathHandle(localMatchIntermediateCS.LMICS.purpleC[i]);

                        if (CompareMonument(localMatchIntermediateCS.LMICS.purpleC[i]))
                        {

                            MonumentValPurple += 1;

                        }
                    }
                }
            }

            LMS.currentTurn += 1;

            LMS.startTurnFunctionOrder[LMS.currentTurn % LMS.TeamCount]();

            LMS.TurnCountText.text = "" + (((LMS.currentTurn - 1) / LMS.TeamCount));
            //Debug.Log(LMS.currentTurn);
            LMS.WinLogic();

            if (WebManage.WManage.MatchType == 0) 
            {
                LMS.SaveMapStateAll();
            }
            else if(WebManage.WManage.MatchType == 1)
            {
                if(WebManage.WManage.SendMapDataBat1Bool == true)
                {
                    WebManage.WManage.SendMapDataBat1Bool = false;
                }
                SendMapDataNetworkBat1();
            }
        }
    }
    void NewTurnLogicBat1()
    {

    }

    public void NewTurn()
    {
        
        LMS.WinConditionObj.SetActive(false);

        if (WebManage.WManage.MatchType == 0) 
        {
            NewTurnLogicBat0();
        }
        else if (WebManage.WManage.MatchType == 1) 
        {
            if (TeamOrderSameAsCurrentTurn())
            {
                NewTurnLogicBat0();
            }
        } // bat type 1
        
    }

    bool TeamOrderSameAsCurrentTurn()
    {
        if(WebManage.WManage.TeamOrder == (LMS.currentTurn % LMS.TeamCount))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void UpdateHealthBar(localMatchIntermediateCS.CharDat HpUpdate)
    {
        LMS.UpdateCharStat();

        float tmp = (float)HpUpdate.Hp / (float)PriWLookup.PWLO.Hp[HpUpdate.PW];
        
        HpUpdate.HealthBar.transform.localScale = (new Vector3(tmp, 1.0f,1.0f)); //

    }

    int AddDamageBasedOnPersonAngle(localMatchIntermediateCS.CharDat Attacker, localMatchIntermediateCS.CharDat Defender)
    {

        GameObject tmp = Instantiate(Attacker.CharObj);

        tmp.transform.position = new Vector3(Attacker.CharObj.transform.position.x, Attacker.CharObj.transform.position.y, Attacker.CharObj.transform.position.z);
        
        tmp.transform.rotation = Quaternion.LookRotation((Attacker.CharObj.transform.position - Defender.CharObj.transform.position)); //may need to use 1 degree or diffrent intervals or raise
        tmp.transform.RotateAround(tmp.transform.position, Vector3.up, -90); //rotate -90 degrees to account for the 90 extra



        float angles = Quaternion.Angle(tmp.transform.rotation, Defender.CharObj.transform.rotation); //technically both are -90 degrees --> Attacker.CharObj.transform.RotateAround(Attacker.CharObj.transform.position, Vector3.up, -90); //rotate -90 degrees to account for the 90 extra

        
        Destroy(tmp);
        //damage for side ways or directly behind attack
        if (angles < 30.0f)
        {
            Debug.Log("4");
            return 4;
        }
        else if(angles >= 30.0f && angles < 135.0f) //meant to be slightly off - but yeah, 90 to "back" is +2 dmg
        {
            Debug.Log("2");
            return 2;
        }
        else
        {
            Debug.Log("0");
            return 0;
        }
    }

    Tuple<int,int, bool> CalculateDamage(localMatchIntermediateCS.CharDat Attacker, localMatchIntermediateCS.CharDat Defender) //returns, damage dealt to enemy, and damage you recive 
    {
        int AttackerEffectiveAtk = 0;
        int AttackerEffectiveDef = 0;

        int DefenderEffectiveAtk = 0;
        int DefenderEffectiveDef = 0;
        int DefenderEffectiveMinRange = 0;
        int DefenderEffectiveMaxRange = 0;

        if (Attacker.CurW == 1)
        {
            AttackerEffectiveAtk = Attacker.Atk;
            AttackerEffectiveDef = Attacker.Def;
        }
        else if(Attacker.CurW == 2)
        {
            AttackerEffectiveAtk = Attacker.Atk2;
            AttackerEffectiveDef = Attacker.Def2;
        }

        if(Defender.CurW == 1)
        {
            DefenderEffectiveAtk = Defender.Atk;
            DefenderEffectiveDef = Defender.Def;
            DefenderEffectiveMinRange = Defender.RngMinPri;
            DefenderEffectiveMaxRange = Defender.RngMaxPri;
        }
        else if (Defender.CurW == 2)
        {
            DefenderEffectiveAtk = Defender.Atk;
            DefenderEffectiveDef = Defender.Def;
            DefenderEffectiveMinRange = Defender.RngMinSec;
            DefenderEffectiveMaxRange = Defender.RngMaxSec;
        }

        int AttackerTileId = localMatchIntermediateCS.LMICS.TilesArray[Attacker.PosY][Attacker.PosX].TileId;
        int DefenderTileId = localMatchIntermediateCS.LMICS.TilesArray[Defender.PosY][Defender.PosX].TileId;

        int AttackerDamage = Math.Max(0, (AttackerEffectiveAtk + Attacker.AtkBuff + TileLookUp.TLU.Tiles[AttackerTileId].Atk + AddDamageBasedOnPersonAngle(Attacker, Defender) ) - (DefenderEffectiveDef + Defender.DefBuff + TileLookUp.TLU.Tiles[DefenderTileId].Def) );
        
        int DefenderDamage = 0;

        int DistOfAttack = Math.Abs((Defender.PosX + Defender.PosY) - (Attacker.PosX + Attacker.PosY));

        bool TooFar = false;

        if (DistOfAttack >= DefenderEffectiveMinRange && DistOfAttack <= DefenderEffectiveMaxRange) {
            DefenderDamage = Math.Max(0, (DefenderEffectiveAtk + Defender.AtkBuff + TileLookUp.TLU.Tiles[DefenderTileId].Atk) - (AttackerEffectiveDef + Attacker.DefBuff + TileLookUp.TLU.Tiles[AttackerTileId].Def));
            
        }
        else
        {
            TooFar = true;
        }

        return Tuple.Create(AttackerDamage, DefenderDamage, TooFar);
    }



    void SetupAttackPrep()
    {
        //public GameObject AttackPrepScreenAttacker;
        //    public GameObject AttackPrepScreenDefender;
        //  public TMP_Text AttackPrepScreenTextAttacker;
        //    public TMP_Text AttackPrepScreenTextDefender;

        Tuple<int, int, bool> BattleResultTmp = CalculateDamage(LMS.SelectedChar, localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat);

        int AttackerEffectiveAtk = 0;
        int AttackerEffectiveDef = 0;

        int DefenderEffectiveAtk = 0;
        int DefenderEffectiveDef = 0;
        
        if(LMS.SelectedChar.CurW == 1)
        {
            AttackerEffectiveAtk = LMS.SelectedChar.Atk;
            AttackerEffectiveDef = LMS.SelectedChar.Def;
        }
        else if (LMS.SelectedChar.CurW == 2)
        {
            AttackerEffectiveAtk = LMS.SelectedChar.Atk2;
            AttackerEffectiveDef = LMS.SelectedChar.Def2;
        }
        if(localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.CurW == 1)
        {
            DefenderEffectiveAtk = localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Atk;
            DefenderEffectiveDef = localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Def;
        }
        else if(localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.CurW == 2)
        {
            DefenderEffectiveAtk = localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Atk2;
            DefenderEffectiveDef = localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Def2;
        }

        LMS.AttackPrepScreenTextAttacker.text = "Attacker:\nAtk: " + (AttackerEffectiveAtk + LMS.SelectedChar.AtkBuff) + "\nDef: " + (AttackerEffectiveDef + LMS.SelectedChar.DefBuff) + "\nDmg: " + BattleResultTmp.Item1 + "\nHp: " + LMS.SelectedChar.Hp + "->" + Math.Max(0, LMS.SelectedChar.Hp - BattleResultTmp.Item2); //print  Atk:, Def, Dmg, HP: [before health]->[after health]
        LMS.AttackPrepScreenTextDefender.text = "Defender:\nAtk: " + (DefenderEffectiveAtk + localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.AtkBuff) + "\nDef: " + (DefenderEffectiveDef + localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.DefBuff) + "\nDmg: " + BattleResultTmp.Item2 + "\nHp: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp + "->" + Math.Max(0, localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp - BattleResultTmp.Item1);
        if(Math.Max(0, localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp - BattleResultTmp.Item1) == 0)
        {

            LMS.AttackPrepScreenTextDefender.text += "\nDie's before counter attack\n";

        }
        if(BattleResultTmp.Item3 == true)
        {

            LMS.AttackPrepScreenTextDefender.text += "\nOut Of Range";

        }

        //print  Atk:, Def, Dmg, HP: [before health]->[after health]

        LMS.AttackPrepScreenAttacker.SetActive(true);
        LMS.StartToAttackButton.SetActive(true);
        LMS.AttackPrepScreenDefender.SetActive(true);
        LMS.CloseActionAtkObj.SetActive(true);

    }

    IEnumerator DiedAnimation(localMatchIntermediateCS.CharDat Cdat, Animator CdatAnim)
    {


        if (CdatAnim.GetBool("Selected") == false)
        {
            CdatAnim.SetBool("Selected", true);
        }

        CdatAnim.SetBool("Dead", true);

        Cdat.HealthBar.SetActive(false);
        Cdat.HealthBarBackLine.SetActive(false);

        yield return null;

        while (CdatAnim.GetBool("Dying") == false)
        {
            yield return null;

        }

    }

    IEnumerator FightAnimation(localMatchIntermediateCS.CharDat Attacker, localMatchIntermediateCS.CharDat Defender, int AtkDealt, int DefDealt, bool OutOfRange, Animator AtkAnim, Animator DefAnim)
    {

        while(DefAnim.GetBool("Selected") == true) //if 2 fight the same guy, delay the animation to not break the code...
        {

            yield return null;


        }
        

        Attacker.CharObj.transform.rotation = Quaternion.LookRotation((Attacker.CharObj.transform.position - Defender.CharObj.transform.position)); //may need to use 1 degree or diffrent intervals or raise
        Attacker.CharObj.transform.RotateAround(Attacker.CharObj.transform.position, Vector3.up, -90); //rotate -90 degrees to account for the 90 extra
        Defender.CharObj.transform.rotation = Quaternion.LookRotation((Defender.CharObj.transform.position - Attacker.CharObj.transform.position)); //may need to use 1 degree or diffrent intervals or raise
        Defender.CharObj.transform.RotateAround(Defender.CharObj.transform.position, Vector3.up, -90); //rotate -90 degrees to account for the 90 extra



        if (AtkAnim.GetBool("Selected") == false)
        {
            AtkAnim.SetBool("Selected", true);
        }
        if (DefAnim.GetBool("Selected") == false)
        {
            DefAnim.SetBool("Selected", true);
        }

        AtkAnim.SetBool("BattleStart", true);
        
        yield return null;
        
        while (AtkAnim.GetBool("BattleStart") )
        {
            //animation sets BattleStart bool false when done
            
            if ( AtkAnim.GetBool("HitTheEnemy") )
            {
                if (AtkDealt > 0)
                {
                    UpdateHealthBar(Defender);

                    DefAnim.SetBool("Hurt", true);

                    //animation sets "Hurt" bool false when done
                }
                else {

                    
                    //TODO: play tink-metal ring sound
                }
                AtkAnim.SetBool("HitTheEnemy", false);

                yield return null;
                
            }
            yield return null;

        }

        while ( DefAnim.GetBool("Hurt") )
        {
            yield return null;
        }

        if (Defender.Hp <= 0)
        {
            //Debug.Log("dead");
            //keep trigger true
            DefAnim.SetBool("Dead", true);

            Defender.HealthBar.SetActive(false);
            Defender.HealthBarBackLine.SetActive(false);

            yield return null;

            while (DefAnim.GetBool("Dying") == false)
            {
                yield return null;

            }
        }



        //if hp<0, play dead you keep laying down, never leave state

        if (OutOfRange == false && Defender.Hp>0)
        {
            DefAnim.SetBool("BattleStart", true);
            //AtkAnim

            yield return null;

            while ( DefAnim.GetBool( "BattleStart" ) ) {

                if (DefAnim.GetBool("HitTheEnemy") )
                {

                    if (DefDealt > 0)
                    {
                        AtkAnim.SetBool("Hurt", true);

                        UpdateHealthBar(Attacker);

                        //animation sets "Hurt" bool false when done
                    }
                    else
                    {


                        //TODO: play tink-metal ring sound
                    }
                    
                    DefAnim.SetBool("HitTheEnemy", false);

                    yield return null;

                }

                yield return null;

            }

            while (AtkAnim.GetBool("Hurt"))
            {
                
                yield return null;

            }

            if (Attacker.Hp <= 0)
            {

                AtkAnim.SetBool("Dead", true);
                Attacker.HealthBar.SetActive(false);
                Attacker.HealthBarBackLine.SetActive(false);

                yield return null;

                while (AtkAnim.GetBool("Dying") == false)
                {
                    yield return null;
                }

            }



            if (AtkAnim.GetBool("Selected") == true)
            {
                AtkAnim.SetBool("Selected", false);
            }
            if (DefAnim.GetBool("Selected") == true)
            {
                DefAnim.SetBool("Selected", false);
            }

            //LMS.currentlyFighting = false;
            yield return null;

        }
        else
        {
            if (AtkAnim.GetBool("Selected") == true)
            {
                AtkAnim.SetBool("Selected", false);
            }
            if (DefAnim.GetBool("Selected") == true)
            {
                DefAnim.SetBool("Selected", false);
            }

            //LMS.currentlyFighting = false;
            yield return null;
        }

        AtkAnim.SetBool("Dying", false);
        DefAnim.SetBool("Dying", false);

        LMS.currentlyFighting = false;
        yield return null;

    }

    public void AttackActionComplete()
    {

        if (LMS.SelectedChar.CurW == 2)
        {

            LMS.SelectedChar.ShowSecondary = true;

        }

        Tuple<int, int, bool> BattleResultTmp = CalculateDamage(LMS.SelectedChar, localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat);

        localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp = Math.Max(0, localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp - BattleResultTmp.Item1);

        if (localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp > 0)
        {
            LMS.SelectedChar.Hp = Math.Max(0, LMS.SelectedChar.Hp - BattleResultTmp.Item2);
        }
        else
        {

            LMS.SelectedChar.Dead = true;

        }


        if(localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Hp > 0)
        {
            //for consistency... I also may need this for abilities...

        }

        else
        {

            localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.Dead = true;

        }


        IEnumerator FA = FightAnimation(LMS.SelectedChar, localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat, BattleResultTmp.Item1, BattleResultTmp.Item2, BattleResultTmp.Item3, LMS.SelectedChar.CharObj.GetComponent<Animator>(), localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedAttackTarget.Item1][LMS.SelectedAttackTarget.Item2].CDat.CharObj.GetComponent<Animator>());

        StartCoroutine(FA);

        //calc damage - deal damage - check if HP<=0 - if less than zero, set char obj to null or a transparent material if I must

        //Couroutine: play animation for attack of player 1 in loop and retrive - if less than 0 HP, you are dead for either, and play death animation - then remove and set false

        LMS.SelectedChar.HasTurn = false;
        
        LMS.AttackPrepScreenAttacker.SetActive(false);
        LMS.StartToAttackButton.SetActive(false);
        LMS.AttackPrepScreenDefender.SetActive(false);
        LMS.CloseActionAtkObj.SetActive(false);


        LMS.SelectedChar.MovedAlready = true;
        
        LMS.currentlyFighting = true;
        
        LMS.CloseActionMenuAndDeselect();

        LMS.SelectedChar = null;

    


    }

    public void CancelBattleButton()
    {
        LMS.attackPrep = false;
        LMS.AttackPrepScreenAttacker.SetActive(false);
        LMS.StartToAttackButton.SetActive(false);
        LMS.AttackPrepScreenDefender.SetActive(false);
        LMS.CloseActionAtkObj.SetActive(false);
        LMS.MovePhase.MovePhase = 0;
        LMS.ClearTmpAtkTiles();
    }
    
    void ClearTmpAbilityTiles()
    {
        foreach (Tuple<int, int> i in LMS.TmpAbilityTiles.Keys)
        {

            Destroy(LMS.TmpAbilityTiles[i]);

        }
        LMS.TmpAbilityTiles.Clear();
    }
    public void CancelAbilityButton()
    {
        LMS.ActionAbilityInfoObj.SetActive(false);
        LMS.CloseActionAbilityObj.SetActive(false);
        LMS.MovePhase.MovePhase = 0;
        LMS.ClearTmpAbilityTiles();
    }

    void AttackCharClick(int y, int x)
    {

        if (LMS.TmpAtkTiles.ContainsKey(Tuple.Create(y, x)) && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat != null && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.Dead == false && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.team != LMS.SelectedChar.team)
        {
            //CancelBattleButton
            LMS.ActionMenuObj.SetActive(false);
            LMS.CloseActionMenuObj.SetActive(false);


            LMS.attackPrep = true;

            LMS.SelectedAttackTarget = Tuple.Create(y, x);

            LMS.SetupAttackPrep();

            //setup attack prep:

            // show attack prep screen with forecast info
            // show check mark to accept battle - in position of regular menu - hide attack prep screen and check mark when CloseActionMenuAndDeselect is called


        }

    }

    IEnumerator handleRunAnim(GameObject tmpObj, Animator tmpObjAnim, int y, int x)
    {
        tmpObjAnim.SetBool("Run", true); //use this object since SelectedChar can now dangurously be changed

        Vector3 PosA = tmpObj.transform.position;
        Vector3 PosB = new Vector3(4 * x, tmpObj.transform.position.y, 4 * y);

        //rotate based on angle from PosA and PosB and then face that direction

        tmpObj.transform.rotation = Quaternion.LookRotation((PosA - PosB)); //may need to use 1 degree or diffrent intervals or raise
        tmpObj.transform.RotateAround(tmpObj.transform.position, Vector3.up, -90); //rotate -90 degrees to account for the 90 extra

        float testc = 0.0f;

        while (testc < 1)
        {
            testc += Time.deltaTime / 0.5f;

            tmpObj.transform.position = Vector3.Lerp(PosA, PosB, testc); //1 second

            yield return null;
        }

        tmpObj.transform.position = PosB;

        tmpObjAnim.SetBool("Run", false); //use this object since SelectedChar can now dangurously be changed

        yield return null;
    }



    void MoveCharClick(int y, int x)
    {
        
        if (LMS.TmpMoveTiles.ContainsKey( Tuple.Create(y, x) ) && (localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat == null || localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.Dead == true) )
        {

            LMS.ClearTmpMoveTiles();
            
            LMS.CalcMoveDone = true;

            LMS.SelectedChar.MovedAlready = true;

            IEnumerator routine = handleRunAnim(LMS.SelectedChar.CharObj, LMS.SelectedChar.CharObj.GetComponent<Animator>(), y, x);
            StartCoroutine(routine);

            localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat = LMS.SelectedChar;

            localMatchIntermediateCS.LMICS.TilesArray[LMS.SelectedChar.PosY][LMS.SelectedChar.PosX].CDat = null;

            LMS.SelectedChar.PosY = y;
            LMS.SelectedChar.PosX = x;

            NewTileStatAdjustAll(LMS.SelectedChar, localMatchIntermediateCS.LMICS.TilesArray);

            NewTurnDeathHandle(LMS.SelectedChar);

            if(LMS.SelectedChar.Hp <= 0)
            {

                CloseActionMenuAndDeselect();

            }
            else
            {
                BasicSendMapDataSTBlocker();
            }

        }

    }

    public void StartMovePhase()
    {

        LMS.MovePhase.MovePhase = 1;

        LMS.CalcMoveDone = false;

    }

    void DFSNextTile(int x, int y, int depth)
    {
        if (localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat != null && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.team != LMS.SelectedChar.team && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat.Dead == false || TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[y][x].TileId].MoveCost == 100) //overlap Person -- or wall, I could use or... but i'm lazy *heh
        {

        }

        else 
        {
            if (depth - TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[y][x].TileId].MoveCost >= 0)
            {
                if (!TmpMoveTiles.ContainsKey(Tuple.Create(y, x)))
                {
                    TmpMoveTiles[Tuple.Create(y, x)] = Instantiate(MiscTileObj.MTO.SelectTile, new Vector3(4 * x, -19.5f, 4 * y), Quaternion.identity);
                }

                MovDFS(x, y, depth - TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[y][x].TileId].MoveCost);  //minus tile ID mov cost
            
            }
        }
    }

    void MovDFS(int x, int y, int depth) //y is initial y pos - x is initial x pos - depth is initial char movment
    {

        if (depth > 0)
        {
            if (x > 0)
            {
                DFSNextTile(x - 1, y, depth);
            }
            if (x < localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0) - 1)
            {
                DFSNextTile(x + 1, y, depth);
            }
            if (y > 0)
            {
                DFSNextTile(x, y - 1, depth);
            }
            if (y < localMatchIntermediateCS.LMICS.TilesArray.GetLength(0) - 1)
            {
                DFSNextTile(x, y + 1, depth);
            }
        }

    }

    void ClearTmpMoveTiles()
    {
        foreach(Tuple<int,int> i in LMS.TmpMoveTiles.Keys)
        {

            Destroy(LMS.TmpMoveTiles[i]);

        }
        LMS.TmpMoveTiles.Clear();
    }

    public void WhereCanIMove()
    {

        if(LMS.CalcMoveDone == false)
        {
            LMS.ActionMenuObj.SetActive(false);

            LMS.MovDFS(LMS.SelectedChar.PosX, LMS.SelectedChar.PosY, LMS.SelectedChar.Mov + LMS.SelectedChar.MovBuff);
            LMS.CalcMoveDone = true;
        }
        
    }

    public void CloseActionMenuAndDeselect()
    {
        LMS.ClearTmpMoveTiles();
        LMS.ClearTmpAtkTiles();
        LMS.ClearTmpAbilityTiles();


        ArrowLineDrawer.Destroy();

        CloseActionRotateObjBackButton.SetActive(false);
        LMS.attackPrep = false;
        LMS.AttackPrepScreenAttacker.SetActive(false);
        LMS.AttackPrepScreenDefender.SetActive(false);
        LMS.StartToAttackButton.SetActive(false);
        LMS.CloseActionAtkObj.SetActive(false);
    
        //hide attack prep screen

        LMS.ActionMenuObj.SetActive(false);

        LMS.CloseActionMenuObj.SetActive(false);

        


        if (LMS.SelectedChar != null)
        {
            if (LMS.currentlyFighting == false && LMS.SelectedChar.CharObj.GetComponent<Animator>().GetBool("Selected") == true)
            {
                LMS.SelectedChar.CharObj.GetComponent<Animator>().SetBool("Selected", false);
            }

            Renderer[] ComponentR;

            ComponentR = LMS.SelectedChar.CharObj.GetComponentsInChildren<Renderer>();

            if (LMS.SelectedChar.MovedAlready == true)
            {//load grey used char mat
                for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                {
                    ComponentR[tmp].material = ConstantCharObject.CCObj.UsedUpMat;
                }
                LMS.SelectedChar.HasTurn = false;
            }
            else
            {
                for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
                {
                    ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[LMS.SelectedChar.team - 1];
                }
            }
        }

        //if()

        LMS.CalcMoveDone = true;

        LMS.MovePhase.MovePhase = 0;

        LMS.SelectedChar = null;

        BasicSendMapDataSTBlocker();
    }

    public void ShowActionMenu(int y, int x)
    {
        if (LMS.SelectedChar != null && LMS.ActionMenuObj.activeSelf == false && LMS.MovePhase.MovePhase == 0 && LMS.SelectedChar.MovedAlready == false && LMS.SelectedChar.HasTurn == true && LMS.attackPrep == false)
        {

            if (SelectedChar.CharObj.GetComponent<Animator>().GetBool("Selected") == false)
            {
                SelectedChar.CharObj.GetComponent<Animator>().SetBool("Selected", true);
            }


            LMS.CloseActionMenuObj.SetActive(true);

            LMS.ActionMenuObj.SetActive(true);

            //clear all options!

            LMS.Attack1.SetActive(true); // TODO: LOAD ATTACK FUNC AND SUCH, SAME FOR MOVE FUNC! 
            LMS.Attack2.SetActive(true);
            if (LMS.SelectedChar.Ability1 != 0)
            {
                LMS.Ability1.SetActive(false);
            }
            else
            {
                LMS.Ability1.SetActive(true);
                //TODO: load ability call func
            }
            if (LMS.SelectedChar.Ability2 != 0)
            {
                LMS.Ability2.SetActive(false);
            }
            else
            {
                LMS.Ability2.SetActive(true);
                //TODO: load ability call func
            }
            LMS.Move.SetActive(true);

            //TODO: Repopulate Menu Items with actions possible
            //check if attack is allowed for Primary - to see if I add - make it sword with 1
            //check if attack is allowed for Secondary - to see if I add - make it sword with 2
            //add move - image of square with arrrow to another square
            //add primary ability slot - dunno... make it a pic with 1
            //add secondary ability slot - dunno... make it a pic with 2


        }

        else if (LMS.SelectedChar != null && LMS.SelectedChar.MovedAlready == true && LMS.SelectedChar.HasTurn == true && LMS.attackPrep == false)
        {
            if (SelectedChar.CharObj.GetComponent<Animator>().GetBool("Selected") == false)
            {
                SelectedChar.CharObj.GetComponent<Animator>().SetBool("Selected", true);
            }


            //clear all options!
            LMS.CloseActionMenuObj.SetActive(true);
            LMS.ActionMenuObj.SetActive(true);

            LMS.Attack1.SetActive(true);
            LMS.Attack2.SetActive(true);
            if (LMS.SelectedChar.Ability1 != 0)
            {
                LMS.Ability1.SetActive(false);
            }
            else
            {
                LMS.Ability1.SetActive(true);
                //TODO: load ability call func
            }
            if (LMS.SelectedChar.Ability2 != 0)
            {
                LMS.Ability2.SetActive(false);
            }
            else
            {
                LMS.Ability2.SetActive(true);
                //TODO: load ability call func
            }
            LMS.Move.SetActive(false);


            //check if attack is allowed for Primary - to see if I add - make it sword with 1
            //check if attack is allowed for Secondary - to see if I add - make it sword with 2
            //add primary ability slot - dunno... make it a pic with 1
            //add secondary ability slot - dunno... make it a pic with 2
        }

        if(LMS.SelectedChar == null || LMS.SelectedChar != null && LMS.SelectedChar.HasTurn == false)
        {

            LMS.ActionMenuObj.SetActive(false);
            LMS.CloseActionMenuObj.SetActive(false);

        }
    }

    void UpdateCharStat()
    {
        if (LMS.HoverTupleStore != null && localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat != null)
        {
            LMS.HoverInfoTMP = localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.name + ":\nHP: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Hp + "/" + PriWLookup.PWLO.Hp[localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.PW] + "\nAttack: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Atk + "\nDefence: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Def + "\nMov: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Mov + "\nPrimary Weapon: " + PriWLookup.PWLO.Name[localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.PW] + "\nRange-Primary: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMinPri + "-" + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMaxPri + "\nAtk (De)Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.AtkBuff + "\nDef (De)Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.DefBuff + "\nMove (De)Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.MovBuff + "\nHp (De)Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.HpBuff;

            if (localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.ShowSecondary == true)
            {
                LMS.HoverInfoTMP += "\nSecondary Weapon: " + SecWLookup.SWLO.Name[localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.SW] + "\nSecondary Atk: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Atk2 + "\nSecondary Def: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Def2 + "\nRange-Secondary: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMinSec + "-" + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMaxSec;
            }

            if (localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Dead == true)
            {
                LMS.HoverInfoTMP += "\n-Dead";
            }
        }
    }

    public void ShowStats(int y, int x)
    {
        if (localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat != null && LMS.PermaHoverInfoTextOff == false || LMS.HoverTupleStore != null && LMS.PermaHoverInfoTextOff == false) //second part never allows to turn off - this changes in the real game with a red x button to cancel out and such
        {
            if (LMS.HoverTupleStore == null || Tuple.Create(y, x) != LMS.HoverTupleStore && localMatchIntermediateCS.LMICS.TilesArray[y][x].CDat != null)
            {

                LMS.HoverTupleStore = Tuple.Create(y, x);

                LMS.HoverInfoTMP = localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.name + ":\nHP: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Hp + "/" + PriWLookup.PWLO.Hp[localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.PW] + "\nAttack: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Atk + "\nDefence: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Def + "\nMov: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Mov + "\nPrimary Weapon: " + PriWLookup.PWLO.Name[localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.PW] + "\nRange-Primary: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMinPri + "-" + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMaxPri + "\nAtk Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.AtkBuff + "\nDef Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.DefBuff + "\nMove Buff: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.MovBuff;

                if (localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.ShowSecondary == true)
                {
                    LMS.HoverInfoTMP += "\nSecondary Weapon: " + SecWLookup.SWLO.Name[localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.SW] + "\nSecondary Atk: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Atk2 + "\nSecondary Def: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Def2 + "\nRange-Secondary: " + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMinSec + "-" + localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.RngMaxSec;
                }

                if (localMatchIntermediateCS.LMICS.TilesArray[LMS.HoverTupleStore.Item1][LMS.HoverTupleStore.Item2].CDat.Dead == true)
                {
                    LMS.HoverInfoTMP += "\n-Dead";
                }


            }
            LMS.HoverInfo.SetActive(true);
            HoverInfoCloseButton.SetActive(true);

            LMS.HoverInfoText.text = LMS.HoverInfoTMP;
        }
        else
        {
            LMS.HoverInfoTMP = "";
            LMS.HoverTupleStore = null;

            LMS.HoverInfo.SetActive(false);
            HoverInfoCloseButton.SetActive(false);
        }

        if(HideTileInfo == false)
        {
            StaticDataMapMaker.controlObj.DrawTileStats(TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[y][x].TileId], TileInfoObjText);
        }
    }

    public void ToggleTileInfoOff()
    {
        
        LMS.HideTileInfo = !LMS.HideTileInfo;
        TileInfoObj.SetActive(!TileInfoObj.activeSelf);

    }

    public void TogglePermaInfoOnOff()
    {

        LMS.PermaHoverInfoTextOff = !LMS.PermaHoverInfoTextOff;

    }
    public void TurnOffInfo()
    {
        LMS.HoverInfoTMP = "";
        LMS.HoverTupleStore = null;
        LMS.HoverInfo.SetActive(false);
        HoverInfoCloseButton.SetActive(false);

    }


    void InverseAtkBuff(localMatchIntermediateCS.CharDat Cdat) //watch this be an ability
    {

        Cdat.AtkBuff *= -1;

    }
    void InverseDefBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.DefBuff *= -1;

    }
    void InverseMovBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.MovBuff *= -1;

    }

    void ClearAtkBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.AtkBuff = 0;

    }
    void ClearDefBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.DefBuff = 0;

    }
    void ClearMovBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.MovBuff = 0;

    }

    void AdjustAtkBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        if (Cdat.AtkBuff > 0)
        {
            Cdat.AtkBuff -= 1;
        }
        else if (Cdat.AtkBuff < 0)
        {
            Cdat.AtkBuff += 1;
        }

    }
    void AdjustDefBuff(localMatchIntermediateCS.CharDat Cdat)
    {
        if (Cdat.DefBuff > 0)
        {
            Cdat.DefBuff -= 1;
        }
        else if (Cdat.DefBuff < 0)
        {
            Cdat.DefBuff += 1;
        }

    }
    void AdjustMovBuff(localMatchIntermediateCS.CharDat Cdat)
    {
        if (Cdat.MovBuff > 0)
        {
            Cdat.MovBuff -= 1;
        }
        else if (Cdat.MovBuff < 0)
        {
            Cdat.MovBuff += 1;
        }

    }
    void AdjustHpBuff(localMatchIntermediateCS.CharDat Cdat)
    {
        if (Cdat.HpBuff > 0)
        {
            Cdat.HpBuff -= 15;

            if (Cdat.HpBuff < 0)
            {
                Cdat.HpBuff = 0;
            }
        }
        else if (Cdat.HpBuff < 0)
        {
            Cdat.HpBuff += 15;

            if (Cdat.HpBuff > 0)
            {
                Cdat.HpBuff = 0;
            }
        }

    }

    void HpBuffHeal(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.Hp += Cdat.HpBuff;

        if(PriWLookup.PWLO.Hp[Cdat.PW] < Cdat.Hp)
        {

            Cdat.Hp = PriWLookup.PWLO.Hp[Cdat.PW];

        }
    }

    void AdjustStatsAll(localMatchIntermediateCS.CharDat Cdat)
    {

        AdjustMovBuff(Cdat);
        AdjustHpBuff(Cdat);
        AdjustDefBuff(Cdat);
        AdjustAtkBuff(Cdat);

    }

    void EndTileAtkBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        if (Cdat.AtkBuff > -6 || Cdat.AtkBuff < 6)
        { //max debuff of 10

            Cdat.AtkBuff += TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId].AtkBuffEnd;
            if (Cdat.AtkBuff > 6)
            {
                Cdat.AtkBuff = 6;
            }
            else if (Cdat.AtkBuff < -6)
            {
                Cdat.AtkBuff = -6;
            }
        }

    }
    void EndTileDefBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        if (Cdat.DefBuff > -6 || Cdat.DefBuff < 6)
        { //max debuff of 10

            Cdat.DefBuff += TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId].DefBuffEnd;
            if (Cdat.DefBuff > 6)
            {
                Cdat.DefBuff = 6;
            }
            else if (Cdat.DefBuff < -6)
            {
                Cdat.DefBuff = -6;
            }
        }

    }
    void EndTileMovBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        if (Cdat.MovBuff > -6 || Cdat.MovBuff < 6)
        { //max debuff of 10

            Cdat.MovBuff += TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId].MovBuffEnd;

            if(Cdat.MovBuff > 6)
            {
                Cdat.MovBuff = 6;
            }
            else if(Cdat.MovBuff < -6)
            {
                Cdat.MovBuff = -6;
            }
        }

    }

    void EndTileHpBuff(localMatchIntermediateCS.CharDat Cdat)
    {

        
   
            Cdat.HpBuff += TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId].HPHeal;

            if (Cdat.HpBuff < -30)
            {
                Cdat.HpBuff = -30;
            }

            if(Cdat.HpBuff > 30)
            {

                Cdat.HpBuff = 30;

            }
     
    }

    void EndTileDmg(localMatchIntermediateCS.CharDat Cdat)
    {

        Cdat.Hp -= TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[Cdat.PosY][Cdat.PosX].TileId].DmgEndTurn;

    }
    //AtkBuffEnd - DefBuffEnd - MovBuffEnd - HPHeal - DmgEndTurn

    void EndTileStatAdjustAll(localMatchIntermediateCS.CharDat Cdat)
    {
        EndTileAtkBuff(Cdat);
        EndTileDefBuff(Cdat);
        EndTileMovBuff(Cdat);
        EndTileHpBuff(Cdat);
        EndTileDmg(Cdat);
        //heal, add heal buf, and then do dmg
    }


    //start turn buffs/debuffs

    public void InitialTileAtkBuff(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        if (Cdat.AtkBuff > -6 || Cdat.AtkBuff < 6)
        { //max debuff of 10

            Cdat.AtkBuff += TileLookUp.TLU.Tiles[TilesArray[Cdat.PosY][Cdat.PosX].TileId].AtkBuffInitial;
            if (Cdat.AtkBuff > 6)
            {
                Cdat.AtkBuff = 6;
            }
            else if (Cdat.AtkBuff < -6)
            {
                Cdat.AtkBuff = -6;
            }
        }

    }
    void InitialTileDefBuff(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        if (Cdat.DefBuff > -6 || Cdat.DefBuff < 6)
        { //max debuff of 10

            Cdat.DefBuff += TileLookUp.TLU.Tiles[TilesArray[Cdat.PosY][Cdat.PosX].TileId].DefBuffInitial;
            if (Cdat.DefBuff > 6)
            {
                Cdat.DefBuff = 6;
            }
            else if (Cdat.DefBuff < -6)
            {
                Cdat.DefBuff = -6;
            }
        }

    }

    void InitialTileDmg(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {

        Cdat.Hp -= TileLookUp.TLU.Tiles[TilesArray[Cdat.PosY][Cdat.PosX].TileId].DmgInitial;

    }

    void NewTileStatAdjustAll(localMatchIntermediateCS.CharDat Cdat, localMatchIntermediateCS.MapMakerVarsDat[][] TilesArray)
    {
        InitialTileAtkBuff(Cdat, TilesArray);
        InitialTileDefBuff(Cdat, TilesArray);
        InitialTileDmg(Cdat, TilesArray);
        //heal, add heal buf, and then do dmg
    }


    //


    //start turn stuff; while true when dying, cannot really ienumerator...

    public void NewTurnDeathHandle(localMatchIntermediateCS.CharDat Cdat)
    {
        UpdateHealthBar(Cdat);

        if (Cdat.Hp <= 0)
        {
            if (Cdat.Dead == false) {

                Cdat.Hp = 0;

                Cdat.Dead = true;

                IEnumerator DA = DiedAnimation(Cdat, Cdat.CharObj.GetComponent<Animator>());

                StartCoroutine(DA);

                Cdat.HasTurn = false;

            }
            else
            {

                Cdat.Hp = 0;

            }
        }
        else
        {


        }
    }

    List<Action> startTurnFunctionOrder = new List<Action>(); //= new List<void>;

    void iterateAbilityCounter1(localMatchIntermediateCS.CharDat CharDat)
    {
        if (CharDat.Ability1 > 0)
        {
            CharDat.Ability1-=1;
        }

    }
    void iterateAbilityCounter2(localMatchIntermediateCS.CharDat CharDat)
    {
        if (CharDat.Ability2 > 0)
        {
            CharDat.Ability2 -= 1;
        }
    }

    void ifLoopTurnTextFix()
    {
        if(LMS.CurrentTeamTurn == 1)
        {
            LMS.WhichColorTurn.text = "blue";
            LMS.WhichColorTurn.color = new Color32(0, 0, 255, 255);
        }
        else if (LMS.CurrentTeamTurn == 2)
        {
            LMS.WhichColorTurn.text = "red";
            LMS.WhichColorTurn.color = new Color32(255, 0, 0, 255);
        }
        else if(LMS.CurrentTeamTurn == 3)
        {
            LMS.WhichColorTurn.text = "green";
            LMS.WhichColorTurn.color = new Color32(0, 255, 0, 255);
        }
        else if(LMS.CurrentTeamTurn == 4)
        {
            LMS.WhichColorTurn.text = "yellow";
            LMS.WhichColorTurn.color = new Color32(255, 255, 0, 255);
        }
        else if(LMS.CurrentTeamTurn == 5)
        {
            LMS.WhichColorTurn.text = "purple";
            LMS.WhichColorTurn.color = new Color32(255, 0, 255, 255);
        }

    }

    void SetYourTurnIdentifier()
    {
        if (WebManage.WManage.MatchType == 1)
        {

            //starts at 1,2,3,4,0 as turn order:
            //Action BlueTurnStartA = BlueTurnStart;
            //Action RedTurnStartA = RedTurnStart;
            //Action YellowTurnStartA = YellowTurnStart;
            //Action GreenTurnStartA = GreenTurnStart;
            //Action PurpleTurnStartA = PurpleTurnStart;

            if (startTurnFunctionOrder[Convert.ToInt32(WebManage.WManage.TeamOrder)] == TSAFuncs[0])
            {
                LMS.SetYourColorThing = true;
                LMS.YourColorTurn.text = "you: blue";
                LMS.YourColorTurn.color = new Color32(0, 0, 255, 255);
            }
            else if (startTurnFunctionOrder[Convert.ToInt32(WebManage.WManage.TeamOrder)] == TSAFuncs[1])
            {
                LMS.SetYourColorThing = true;
                LMS.YourColorTurn.text = "you: red";
                LMS.YourColorTurn.color = new Color32(255, 0, 0, 255);
            }

            else if (startTurnFunctionOrder[Convert.ToInt32(WebManage.WManage.TeamOrder)] == TSAFuncs[2]) //yellow
            {
                LMS.SetYourColorThing = true;
                LMS.YourColorTurn.text = "you: yellow";
                LMS.YourColorTurn.color = new Color32(255, 255, 0, 255);
            }

            else if (startTurnFunctionOrder[Convert.ToInt32(WebManage.WManage.TeamOrder)] == TSAFuncs[3]) //green - need to fix this inconsistancy of green and yellow flip in the manager only
            {
                LMS.SetYourColorThing = true;
                LMS.YourColorTurn.text = "you: green";
                LMS.YourColorTurn.color = new Color32(0, 255, 0, 255);
            }

            else if (startTurnFunctionOrder[Convert.ToInt32(WebManage.WManage.TeamOrder)] == TSAFuncs[4])
            {
                LMS.SetYourColorThing = true;
                LMS.YourColorTurn.text = "you: purple";
                LMS.YourColorTurn.color = new Color32(255, 0, 255, 255);
            }
        }
    }

    void BlueTurnStart()
    {
        Debug.Log("Blue New Turn");
        LMS.CurrentTeamTurn = 1;

        for (int i = 0; i < localMatchIntermediateCS.LMICS.blueC.Count; i++) 
        {
            localMatchIntermediateCS.LMICS.blueC[i].HasTurn = true;

            AdjustAtkBuff(localMatchIntermediateCS.LMICS.blueC[i]);
            AdjustDefBuff(localMatchIntermediateCS.LMICS.blueC[i]);
            AdjustMovBuff(localMatchIntermediateCS.LMICS.blueC[i]);

            iterateAbilityCounter1(localMatchIntermediateCS.LMICS.blueC[i]);
            iterateAbilityCounter2(localMatchIntermediateCS.LMICS.blueC[i]);
        }

        LMS.WhichColorTurn.text = "blue";
        LMS.WhichColorTurn.color = new Color32(0, 0, 255, 255);


    }
    void RedTurnStart()
    {
        Debug.Log("Red New Turn");
        LMS.CurrentTeamTurn = 2;

        for (int i = 0; i < localMatchIntermediateCS.LMICS.redC.Count; i++)
        {
            localMatchIntermediateCS.LMICS.redC[i].HasTurn = true;

            AdjustAtkBuff(localMatchIntermediateCS.LMICS.redC[i]);
            AdjustDefBuff(localMatchIntermediateCS.LMICS.redC[i]);
            AdjustMovBuff(localMatchIntermediateCS.LMICS.redC[i]);

            iterateAbilityCounter1(localMatchIntermediateCS.LMICS.redC[i]);
            iterateAbilityCounter2(localMatchIntermediateCS.LMICS.redC[i]);
        }

        LMS.WhichColorTurn.text = "red";
        LMS.WhichColorTurn.color = new Color32(255, 0, 0, 255);


    }
    void YellowTurnStart() // I flipped the things... i'm dumb... meh - green
    {
        Debug.Log("Green New Turn");
        LMS.CurrentTeamTurn = 3;

        for (int i = 0; i < localMatchIntermediateCS.LMICS.yellowC.Count; i++)
        {
            localMatchIntermediateCS.LMICS.yellowC[i].HasTurn = true;

            AdjustAtkBuff(localMatchIntermediateCS.LMICS.yellowC[i]);
            AdjustDefBuff(localMatchIntermediateCS.LMICS.yellowC[i]);
            AdjustMovBuff(localMatchIntermediateCS.LMICS.yellowC[i]);

            iterateAbilityCounter1(localMatchIntermediateCS.LMICS.yellowC[i]);
            iterateAbilityCounter2(localMatchIntermediateCS.LMICS.yellowC[i]);
        }
        
        LMS.WhichColorTurn.text = "green";
        LMS.WhichColorTurn.color = new Color32(0, 255, 0, 255);
        
    }
    void GreenTurnStart() // // I flipped the things... i'm dumb... meh - yellow
    {
        Debug.Log("Yellow New Turn");
        LMS.CurrentTeamTurn = 4;

        for (int i = 0; i < localMatchIntermediateCS.LMICS.greenC.Count; i++)
        {
            localMatchIntermediateCS.LMICS.greenC[i].HasTurn = true;

            AdjustAtkBuff(localMatchIntermediateCS.LMICS.greenC[i]);
            AdjustDefBuff(localMatchIntermediateCS.LMICS.greenC[i]);
            AdjustMovBuff(localMatchIntermediateCS.LMICS.greenC[i]);

            iterateAbilityCounter1(localMatchIntermediateCS.LMICS.greenC[i]);
            iterateAbilityCounter2(localMatchIntermediateCS.LMICS.greenC[i]);
        }

        LMS.WhichColorTurn.text = "yellow";
        LMS.WhichColorTurn.color = new Color32(255, 255, 0, 255);

        
    }
    void PurpleTurnStart()
    {
        Debug.Log("Purple New Turn");
        LMS.CurrentTeamTurn = 5;

        for (int i = 0; i < localMatchIntermediateCS.LMICS.purpleC.Count; i++)
        {
            localMatchIntermediateCS.LMICS.purpleC[i].HasTurn = true;

            AdjustAtkBuff(localMatchIntermediateCS.LMICS.purpleC[i]);
            AdjustDefBuff(localMatchIntermediateCS.LMICS.purpleC[i]);
            AdjustMovBuff(localMatchIntermediateCS.LMICS.purpleC[i]);

            iterateAbilityCounter1(localMatchIntermediateCS.LMICS.purpleC[i]);
            iterateAbilityCounter2(localMatchIntermediateCS.LMICS.purpleC[i]);
        }

        LMS.WhichColorTurn.text = "purple";
        LMS.WhichColorTurn.color = new Color32(255, 0, 255, 255);

        
    }


    private UnityTemplateProjects.SimpleCameraController.CameraState CamS;

    void CharInitializer(localMatchIntermediateCS.CharDat CharData)
    {
        if (CharData.Dead == false)
        {
            CharData.CharObj = Instantiate(ConstantCharObject.CCObj.CharObjConstant, new Vector3(4 * CharData.PosX, -19, 4 * CharData.PosY), Quaternion.identity);
            CharData.HatObj = Instantiate(HatLookup.HLO.Hat[CharData.CH], new Vector3(0, 0, 0), Quaternion.identity);

            if (CharData.CurW == 1)
            {
                CharData.WeaponObj = Instantiate(PriWLookup.PWLO.PriWeapons[CharData.PW], new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (CharData.CurW == 2)
            {
                CharData.WeaponObj = Instantiate(SecWLookup.SWLO.SecWeapons[CharData.SW], new Vector3(0, 0, 0), Quaternion.identity);
            }

            ParentConstraint pc3 = CharData.WeaponObj.AddComponent<ParentConstraint>();
            ConstraintSource constraintSource2 = new ConstraintSource();
            constraintSource2.sourceTransform = CharData.CharObj.transform.Find(ConstantCharObject.CCObj.WHandString);
            constraintSource2.weight = 1;
            pc3.AddSource(constraintSource2);
            pc3.constraintActive = true;
            //pc1.rotationAxis = Axis.None;
            pc3.SetTranslationOffset(0, new Vector3(-1.0f, 0.0f, 0.0f));
            pc3.SetRotationOffset(0, new Vector3(270.0f, 90.0f, 0.0f));


            CharData.HealthBar = Instantiate(ConstantCharObject.CCObj.Bar, new Vector3(4 * CharData.PosX, -17, 4 * CharData.PosY), Quaternion.identity);
            CharData.HealthBarBackLine = Instantiate(ConstantCharObject.CCObj.Bar, new Vector3(4 * CharData.PosX, -17, 4 * CharData.PosY), Quaternion.identity);

            //https://docs.unity3d.com/Manual/class-ParentConstraint.html - to link without parent and scale affect


            CharData.HealthBar.GetComponentsInChildren<Renderer>()[0].material = ConstantCharObject.CCObj.matArrHard[CharData.team - 1];
            CharData.HealthBarBackLine.GetComponentsInChildren<Renderer>()[0].material = ConstantCharObject.CCObj.BackBar;

            ParentConstraint pc1 = CharData.HealthBar.AddComponent<ParentConstraint>();
            ParentConstraint pc2 = CharData.HealthBarBackLine.AddComponent<ParentConstraint>();

            ConstraintSource constraintSource = new ConstraintSource();

            constraintSource.sourceTransform = CharData.CharObj.transform;
            constraintSource.weight = 1;

            pc1.AddSource(constraintSource);

            pc2.AddSource(constraintSource);

            pc1.constraintActive = true;
            pc2.constraintActive = true;

            pc1.weight = 1;
            pc2.weight = 1;

            //pc1.rotationAxis = Axis.None;
            //pc2.rotationAxis = Axis.None;

            pc1.SetTranslationOffset(0, new Vector3(1.0f, 3.5f, 0.5f));
            pc2.SetTranslationOffset(0, new Vector3(1.0f, 3.4f, 0.5f));

            Transform head;

            head = CharData.CharObj.transform.Find(ConstantCharObject.CCObj.headString);

            CharData.HatObj.transform.SetParent(head, false);

            //Fix size
            CharData.CharObj.transform.localScale += new Vector3(-0.5f, -0.5f, -0.5f);

            //SetColor
            Renderer[] ComponentR;
            ComponentR = CharData.CharObj.GetComponentsInChildren<Renderer>();
            for (int tmp = 0; tmp < ComponentR.GetLength(0); tmp++)
            {

                if (CharData.HasTurn == false && LMS.CurrentTeamTurn == CharData.team)
                {
                    ComponentR[tmp].material = ConstantCharObject.CCObj.UsedUpMat;
                }
                else
                {
                    ComponentR[tmp].material = ConstantCharObject.CCObj.matArr[CharData.team - 1];
                }
            }

            CharData.anim = CharData.CharObj.GetComponent<Animator>();

            if (CharData.CurW == 1)
            {
                localMatchIntermediateCS.LMICS.ChooseAndSetAnim(PriWLookup.PWLO.MainAnimNum[CharData.PW], CharData.anim);
            }
            else if (CharData.CurW == 2)
            {
                localMatchIntermediateCS.LMICS.ChooseAndSetAnim(SecWLookup.SWLO.MainAnimNum[CharData.SW], CharData.anim);
            }
        }
    }
    

    void SomeBasicMapSetup()
    {
        LMS.TeamCount = 0;

        if(localMatchIntermediateCS.LMICS.blueC.Count != 0) { LMS.TeamCount += 1; }
        if (localMatchIntermediateCS.LMICS.redC.Count != 0) {  LMS.TeamCount += 1; }
        if (localMatchIntermediateCS.LMICS.yellowC.Count != 0) {  LMS.TeamCount += 1; }
        if (localMatchIntermediateCS.LMICS.greenC.Count != 0) {  LMS.TeamCount += 1; }
        if (localMatchIntermediateCS.LMICS.purpleC.Count != 0) {  LMS.TeamCount += 1; }

        TSAFuncs.Add(BlueTurnStart);
        TSAFuncs.Add(RedTurnStart);
        TSAFuncs.Add(GreenTurnStart);
        TSAFuncs.Add(YellowTurnStart);
        TSAFuncs.Add(PurpleTurnStart);

        if (localMatchIntermediateCS.LMICS.blueC.Count != 0)
        {
            startTurnFunctionOrder.Add(TSAFuncs[0]);
            
            Debug.Log("BLUE: " + localMatchIntermediateCS.LMICS.blueC.Count);


        }

        if (localMatchIntermediateCS.LMICS.redC.Count != 0)
        {
            startTurnFunctionOrder.Add(TSAFuncs[1]);

            Debug.Log("RED: " + localMatchIntermediateCS.LMICS.redC.Count);


        }

        if (localMatchIntermediateCS.LMICS.greenC.Count != 0)
        {
            startTurnFunctionOrder.Add(TSAFuncs[2]);
            
            Debug.Log("YELLOW: " + localMatchIntermediateCS.LMICS.greenC.Count);


        }

        if (localMatchIntermediateCS.LMICS.yellowC.Count != 0) //yes this is flipped
        {
            startTurnFunctionOrder.Add(TSAFuncs[3]);
            
            Debug.Log("GREEN: " + localMatchIntermediateCS.LMICS.yellowC.Count);


        }

        if (localMatchIntermediateCS.LMICS.purpleC.Count != 0)
        {
            startTurnFunctionOrder.Add(TSAFuncs[4]);
            
            Debug.Log("PURPLE: " + localMatchIntermediateCS.LMICS.purpleC.Count);


        }

        SetYourTurnIdentifier();
        

        //startTurnFunctionOrder
        if (StaticDataMapMaker.controlObj.LoadMapSaveDat == true)
        {
            StaticDataMapMaker.controlObj.LoadMapSaveDat = false;
        }
        else
        {
            startTurnFunctionOrder[currentTurn % TeamCount]();
        }

        LMS.TurnCountText.text = "" + (((LMS.currentTurn - 1) / LMS.TeamCount));

        DrawGrid.GridLines.UpdateDrawGridWidthSolidVal(localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0));
        DrawGrid.GridLines.UpdateDrawGridHeightSolidVal(localMatchIntermediateCS.LMICS.TilesArray.GetLength(0));
        DrawGrid.GridLines.UpdateGridDraw();


    }
    void Awake()
    {

        if (localMatchIntermediateCS.LMICS == null)
        {
            Instantiate(LMICSObjMidLoad, new Vector3(0, 0, 0), Quaternion.identity);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        LMS = this;
        
        Debug.Log(Application.persistentDataPath);

        CamS = Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState;

        if (StaticDataMapMaker.controlObj.LoadMapSaveDat)
        {

            if(WebManage.WManage.MatchType == 0){
            //StaticDataMapMaker.controlObj.LoadMapSaveDat = false;
                LoadMapStateAll();

            }
            else if(WebManage.WManage.MatchType == 1)
            {
                LMS.LoadMapStateAllLogic(WebManage.WManage.JsonReceiveS.s);   
            }

            StaticDataMapMaker.controlObj.LoadMapSaveDat = false;
        }
        else
        {
            for (int i = 0; i < localMatchIntermediateCS.LMICS.blueC.Count; i++)
            {
                CharInitializer(localMatchIntermediateCS.LMICS.blueC[i]);
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.redC.Count; i++)
            {
                CharInitializer(localMatchIntermediateCS.LMICS.redC[i]);
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.yellowC.Count; i++)
            {
                CharInitializer(localMatchIntermediateCS.LMICS.yellowC[i]);
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.greenC.Count; i++)
            {
                CharInitializer(localMatchIntermediateCS.LMICS.greenC[i]);
            }
            for (int i = 0; i < localMatchIntermediateCS.LMICS.purpleC.Count; i++)
            {
                CharInitializer(localMatchIntermediateCS.LMICS.purpleC[i]);
            }

            for (int i = 0; i < localMatchIntermediateCS.LMICS.TilesArray.GetLength(0); i++)
            {
                for (int ii = 0; ii < localMatchIntermediateCS.LMICS.TilesArray[i].GetLength(0); ii++)
                {
                    localMatchIntermediateCS.LMICS.TilesArray[i][ii].Tile = Instantiate(TileLookUp.TLU.Tiles[localMatchIntermediateCS.LMICS.TilesArray[i][ii].TileId].Obj, new Vector3(4 * ii, -20, 4 * i), Quaternion.identity);
                }
            }

            currentTurn += 1;

        }

        SomeBasicMapSetup();
    
    }

    void matchType0Logic()
    {

        if (localMatchIntermediateCS.LMICS != null)
        {
            if (CamS.x < 0)
            {

                CamS.x = 1;

            }

            if (CamS.x > 4 * localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0))
            {

                CamS.x = 4 * localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0) - 1;

            }


            if (CamS.z < 0)
            {

                CamS.z = 1;

            }

            if (CamS.z > 4 * localMatchIntermediateCS.LMICS.TilesArray.GetLength(0))
            {

                CamS.z = 4 * localMatchIntermediateCS.LMICS.TilesArray.GetLength(0) - 1;

            }

            Vector3 tmpVec3 = LightRayToCoords.LRC.GetRayCamMouseXYZ();

            LMS.CXPos = Convert.ToInt32((Math.Abs(tmpVec3.x / 4)));
            LMS.CYPos = Convert.ToInt32((Math.Abs(tmpVec3.z / 4)));


            if (LMS.CXPos < 0 || LMS.CXPos > localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0))
            {
                LMS.CXPos = 0;
            }
            if (LMS.CYPos < 0 || LMS.CYPos > localMatchIntermediateCS.LMICS.TilesArray.GetLength(0))
            {
                LMS.CYPos = 0;
            }

            if (LMS.MovePhase.MovePhase == 0)
            {
                if (Input.GetMouseButton(0))
                {
                    if (localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat != null && localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat.Dead == false && LMS.CurrentTeamTurn == localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat.team && localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat.HasTurn == true)
                    { //double barrier check for 'just in case'

                        Renderer[] ComponentR;

                        if (SelectedChar != null)
                        {

                            ComponentR = LMS.SelectedChar.CharObj.GetComponentsInChildren<Renderer>();
                            for (int i = 0; i < ComponentR.GetLength(0); i++)
                            {
                                ComponentR[i].material = ConstantCharObject.CCObj.matArr[SelectedChar.team - 1];
                            }
                        }

                        SelectedChar = localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat;

                        ComponentR = LMS.SelectedChar.CharObj.GetComponentsInChildren<Renderer>();
                        for (int i = 0; i < ComponentR.GetLength(0); i++)
                        {
                            ComponentR[i].material = ConstantCharObject.CCObj.Selected;
                        }

                        if (SelectedChar.CharObj.GetComponent<Animator>().GetBool("Selected") == false)
                        {
                            SelectedChar.CharObj.GetComponent<Animator>().SetBool("Selected", true);
                        }

                    }


                }
            }

            else if (LMS.MovePhase.MovePhase == 1 && tmpVec3.x != -10000)
            {

                LMS.WhereCanIMove();

                if (Input.GetMouseButton(0))
                {
                    LMS.MoveCharClick(LMS.CYPos, LMS.CXPos);
                }
            }

            else if (LMS.MovePhase.MovePhase == 2 && tmpVec3.x != -10000)
            {

                if (Input.GetMouseButton(0))
                {
                    LMS.AttackCharClick(LMS.CYPos, LMS.CXPos);
                }

            }

            else if (LMS.MovePhase.MovePhase == 3 && tmpVec3.x != -10000)
            {
                DrawRayDirection(4 * SelectedChar.PosX, 4 * SelectedChar.PosY, LMS.CXPos * 4, LMS.CYPos * 4);

                if (Input.GetMouseButton(0))
                {
                    LMS.rotateCharTheRotation(LMS.CXPos, LMS.CYPos);
                    ArrowLineDrawer.Destroy();
                }


            }

            else if (LMS.MovePhase.MovePhase == 4 && tmpVec3.x != -10000)
            {

                if (Input.GetMouseButton(0))
                {
                    LMS.AbilityClickLogic(LMS.CXPos, LMS.CYPos);
                    //    LMS.rotateCharTheRotation(LMS.CXPos, LMS.CYPos);
                    //    ArrowLineDrawer.Destroy();
                }


            }
            else if (LMS.MovePhase.MovePhase == 5)
            {

                if (SelectedChar.HasTurn == false)
                {
                    CloseActionMenuAndDeselect();
                    Debug.Log("yeay");
                }


            } // middle of ability command

            LMS.ShowStats(LMS.CYPos, LMS.CXPos);
            LMS.ShowActionMenu(LMS.CYPos, LMS.CXPos);

        }

    }

    void BasicSendMapDataSTBlocker()
    {
        if (WebManage.WManage.MatchType == 1)
        {
            if (TeamOrderSameAsCurrentTurn())
            {
                // need to add in the middle checks for loading map data before action?!!?
                if (WebManage.WManage.SendMapDataBat1Bool == false)
                {
                    SendMapDataNetworkBat1();
                }
            }
        }
    }

    void matchType1Logic()
    {
        //Debug.Log(LMS.currentTurn);
        if (TeamOrderSameAsCurrentTurn())
        {

        }
        else{
            //CheckAndLoadWebMapBat1();
            CheckAndLoadWebMapALLBat1();
        }

        if (localMatchIntermediateCS.LMICS != null)
        {
            if (CamS.x < 0)
            {

                CamS.x = 1;

            }

            if (CamS.x > 4 * localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0))
            {

                CamS.x = 4 * localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0) - 1;

            }


            if (CamS.z < 0)
            {

                CamS.z = 1;

            }

            if (CamS.z > 4 * localMatchIntermediateCS.LMICS.TilesArray.GetLength(0))
            {

                CamS.z = 4 * localMatchIntermediateCS.LMICS.TilesArray.GetLength(0) - 1;

            }

            Vector3 tmpVec3 = LightRayToCoords.LRC.GetRayCamMouseXYZ();

            LMS.CXPos = Convert.ToInt32((Math.Abs(tmpVec3.x / 4)));
            LMS.CYPos = Convert.ToInt32((Math.Abs(tmpVec3.z / 4)));


            if (LMS.CXPos < 0 || LMS.CXPos > localMatchIntermediateCS.LMICS.TilesArray[0].GetLength(0))
            {
                LMS.CXPos = 0;
            }
            if (LMS.CYPos < 0 || LMS.CYPos > localMatchIntermediateCS.LMICS.TilesArray.GetLength(0))
            {
                LMS.CYPos = 0;
            }

            if (TeamOrderSameAsCurrentTurn())
            {
                if (LMS.MovePhase.MovePhase == 0)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat != null && localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat.Dead == false && LMS.CurrentTeamTurn == localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat.team && localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat.HasTurn == true)
                        { //double barrier check for 'just in case'
                            BasicSendMapDataSTBlocker();

                            Renderer[] ComponentR;

                            if (SelectedChar != null)
                            {

                                ComponentR = LMS.SelectedChar.CharObj.GetComponentsInChildren<Renderer>();
                                for (int i = 0; i < ComponentR.GetLength(0); i++)
                                {
                                    ComponentR[i].material = ConstantCharObject.CCObj.matArr[SelectedChar.team - 1];
                                }
                            }

                            SelectedChar = localMatchIntermediateCS.LMICS.TilesArray[CYPos][CXPos].CDat;

                            ComponentR = LMS.SelectedChar.CharObj.GetComponentsInChildren<Renderer>();
                            for (int i = 0; i < ComponentR.GetLength(0); i++)
                            {
                                ComponentR[i].material = ConstantCharObject.CCObj.Selected;
                            }

                            if (SelectedChar.CharObj.GetComponent<Animator>().GetBool("Selected") == false)
                            {
                                SelectedChar.CharObj.GetComponent<Animator>().SetBool("Selected", true);
                            }

                        }


                    }
                }

                else if (LMS.MovePhase.MovePhase == 1 && tmpVec3.x != -10000)
                {

                    LMS.WhereCanIMove();

                    if (Input.GetMouseButton(0))
                    {
                        LMS.MoveCharClick(LMS.CYPos, LMS.CXPos);
                    }
                }

                else if (LMS.MovePhase.MovePhase == 2 && tmpVec3.x != -10000)
                {

                    if (Input.GetMouseButton(0))
                    {
                        LMS.AttackCharClick(LMS.CYPos, LMS.CXPos);
                    }

                }

                else if (LMS.MovePhase.MovePhase == 3 && tmpVec3.x != -10000)
                {
                    DrawRayDirection(4 * SelectedChar.PosX, 4 * SelectedChar.PosY, LMS.CXPos * 4, LMS.CYPos * 4);

                    if (Input.GetMouseButton(0))
                    {
                        LMS.rotateCharTheRotation(LMS.CXPos, LMS.CYPos);
                        ArrowLineDrawer.Destroy();
                    }


                }

                else if (LMS.MovePhase.MovePhase == 4 && tmpVec3.x != -10000)
                {

                    if (Input.GetMouseButton(0))
                    {
                        LMS.AbilityClickLogic(LMS.CXPos, LMS.CYPos);
                        //    LMS.rotateCharTheRotation(LMS.CXPos, LMS.CYPos);
                        //    ArrowLineDrawer.Destroy();
                    }


                }
                else if (LMS.MovePhase.MovePhase == 5)
                {

                    if (SelectedChar.HasTurn == false)
                    {
                        CloseActionMenuAndDeselect();
                        Debug.Log("yeay");
                    }
                    else
                    {
                        BasicSendMapDataSTBlocker();
                    }

                } // middle of ability command

                LMS.ShowActionMenu(LMS.CYPos, LMS.CXPos);

            }

            LMS.ShowStats(LMS.CYPos, LMS.CXPos);


        }

    }



    // Update is called once per frame
    void Update()
    {
        if(WebManage.WManage.MatchType == 0)
        {
            matchType0Logic();
        }
        else if(WebManage.WManage.MatchType == 1)
        {
            matchType1Logic();
        }
        
    }
}
