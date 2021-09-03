using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;



public class StaticDataMapMaker : MonoBehaviour
{
    //StaticDataMapMaker.controlObj

    int ClientID;

    int GameModeType = 0; //standart non online
                          //1 means random match online without save data - only timer and online
                          //2 is long match 
                          //3 is ?


    public void DrawTileStats(TileLookUp.TilesC tile, TMP_Text TextMod)
    {
        TextMod.text = "Tile: " + tile.name + "\nAtk: " + tile.Atk + "\nDef: " + tile.Def + "\nMov: " + tile.MoveCost;
        //add if and  += for other stats here

        if (tile.AtkBuffInitial != 0)
        {

            TextMod.text += "\nAtk Buff [StandOn]: " + tile.AtkBuffInitial;

        }

        if (tile.AtkBuffEnd != 0)
        {

            TextMod.text += "\nAtk Buff [EndTurn]: " + tile.AtkBuffEnd;

        }
        if (tile.DefBuffInitial != 0)
        {

            TextMod.text += "\nDef Buff [StandOn]: " + tile.DefBuffInitial;

        }
        if (tile.DefBuffEnd != 0)
        {

            TextMod.text += "\nDef Buff [EndTurn]: " + tile.DefBuffEnd;

        }
        if (tile.MovBuffEnd != 0)
        {

            TextMod.text += "\nMov Buff [EndTurn]: " + tile.MovBuffEnd;

        }
        if (tile.HPHeal != 0)
        {

            TextMod.text += "\nHp heal [EndTurn-%]: " + tile.HPHeal;

        }
        if (tile.DmgInitial != 0)
        {

            TextMod.text += "\nDmg [StandOn]: " + tile.DmgInitial;

        }
        if (tile.DmgEndTurn != 0)
        {

            TextMod.text += "\nDmg [EndTurn]: " + tile.DmgEndTurn;

        }

    }


    public void LoadDownloadSaveMapName()
    {
            SceneManager.LoadScene("DownloadSaveMapName");
    }

    public void LoadDownloadShowOff()
    {
            SceneManager.LoadScene("DownloadShowOff");
    }
    
    public void LoadDownloadShowOffReset()
    {
        StaticDataMapMaker.controlObj.LoadMapSaveDat = true;

        SceneManager.LoadScene("DownloadShowOff");
    }

    public void LoadDownloadMapBase()
    {
            SceneManager.LoadScene("DownloadMapBase");
    }

    public void LoadShowOffToRemoveUploadedMap()
    {
            SceneManager.LoadScene("ShowOffToRemoveUploadedMap");
    }

    public void LoadShowOffToDeleteLocal()
    {
        SceneManager.LoadScene("ShowOffToDeleteLocal");
    }

    public void LoadSelectUploadedMapDelete()
    {
        SceneManager.LoadScene("SelectUploadedMapDelete");
    }

    public void LoadSelectDeleteLocal()
    {
        SceneManager.LoadScene("SelectDeleteLocal");
    }

    public void LoadFriendliesGetMapName()
    {
        if(WebManage.WManage.id !=0)
        {
        SceneManager.LoadScene("MapNameFriendlies");
        }
    }

    public void LoadRandBattle()
    {
        if (WebManage.WManage.id != 0)
        {
            WebManage.WManage.RandomMatch = true;
            SceneManager.LoadScene("Rand_Battle_Wait");
        }
    }

    public void LoadBattleContinueLoad()
    {
        if(WebManage.WManage.id != 0){
            SceneManager.LoadScene("ContinueBattleLoad");
        }
    }

    public void LoadSelectUploadMap()
    {
        if(WebManage.WManage.id != 0)
        {
            SceneManager.LoadScene("SelectUploadMap");
        }
    }

    public void LoadLastSelectNameUploadMap()
    {
        if(WebManage.WManage.id != 0)
        {
            SceneManager.LoadScene("LastSelectNameUploadMap");
        }
    
    }

    public void LoadPreShowUploadAcceptBackMap()
    {
        if(WebManage.WManage.id != 0)
        {
            SceneManager.LoadScene("ShowOffBackAndUploadAllow");
        }
    }

    public void LoadLoginMenu()
    {
        if(WebManage.WManage.id != 0){
            SceneManager.LoadScene("Login");
        }
    }

    public void LoadMain()
    {
        WebManage.WManage.FoundMatch = false;
        WebManage.WManage.ResetVarsNotID();
        SceneManager.LoadScene("Main");

    }

    public void LoadFriendJoin()
    {
        if(WebManage.WManage.id != 0)
        {
        SceneManager.LoadScene("Join_Friend");
        }
    }

    public void LoadFriendMakeMatchGetMap()
    {
        SceneManager.LoadScene("SelectMapForFriendMatch");
    }

    public void LoadMapMakerChoiceScene()
    {
        SceneManager.LoadScene("MapMakerChoiceScene");
    }
    

    public void LoadBattleChoices()
    {
        SceneManager.LoadScene("BattleChoices");
    }

    public void LoadFriendliesBattleTypes(){
        
        SceneManager.LoadScene("FriendBatChoice");

    }

    public void LoadMainClearRandomMatch()
    {
        WebManage.WManage.ResetVarsNotID();
        StaticDataMapMaker.controlObj.LoadMapDatPath = "";

        SceneManager.LoadScene("Main");

    }
    public void LoadMapEditSelectScene()
    {

        SceneManager.LoadScene("LoadMapEdit");

    }
    public void LoadCharacterMakerSelectScene()
    {

        SceneManager.LoadScene("CharacterMakerSelectScene");

    }
    public void LoadCharacterMakerMaker()
    {

        SceneManager.LoadScene("CharacterMakerMaker");

    }
    public void LoadLocalMatchMapSelect()
    {

        SceneManager.LoadScene("LocalMatchMapSelect");

    }

    public void LoadMapNameLocalMatchSelect()
    {

        SceneManager.LoadScene("MapNameLocalMatchSelect");

    }

    public void LoadMidMapLocalFight()
    {

        SceneManager.LoadScene("LocalMatchMapMidWayLoad");

    }

    
    public static StaticDataMapMaker controlObj;
    
    public string CharPassName = "";
    public int CharPassSW = 0;
    public int CharPassPW = 0;
    public int CharPassCH = 0;
    public int CharPassCB = 0;


    public string MapName = "NULL";
    public int MapWidth = 10;
    public int MapHeight = 10;
    
    public int DrawSTile = 0;

    public bool LoadMapDat = false;
    public string LoadMapDatPath;


    public bool LoadMapSaveDat = false;//?
    public string saveMapDatString;
    public string loadMapDatStringLocalMatch; 

    public void PostDraw()
    {

        DrawTileStats(TileLookUp.TLU.Tiles[controlObj.DrawSTile], MapMakerMakerScript.MMSS.TileInfoText);

    }



    public void ClearCharSelect()
    {
        controlObj.DrawSTile = 0;
        
        MapMakerMakerScript.MMSS.TileInfoText.text = "Click to remove \nspawn position";

    }
    public void BlueCharSelect()
    {
        controlObj.DrawSTile = 1;
        MapMakerMakerScript.MMSS.TileInfoText.text = "Click To add spawn \nposition for blue players";

    }
    public void RedCharSelect()
    {
        controlObj.DrawSTile = 2;
        MapMakerMakerScript.MMSS.TileInfoText.text = "Click To add spawn \nposition for red players";

    }
    public void GreenCharSelect()
    {
        controlObj.DrawSTile = 3;
        MapMakerMakerScript.MMSS.TileInfoText.text = "Click To add spawn \nposition for green players";

    }
    public void YellowCharSelect()
    {
        controlObj.DrawSTile = 4;
        MapMakerMakerScript.MMSS.TileInfoText.text = "Click To add spawn \nposition for yellow players";

    }
    public void PurpleCharSelect()
    {
        controlObj.DrawSTile = 5;
        MapMakerMakerScript.MMSS.TileInfoText.text = "Click To add spawn \nposition for purple players";
    }


    public void Forest1Select()
    {
        controlObj.DrawSTile = 6;

        PostDraw();
    }
    public void Forest2Select()
    {
        controlObj.DrawSTile = 7;

        PostDraw();

    }

    public void Forest3Select()
    {

        controlObj.DrawSTile = 8;

        PostDraw();
    }
    public void Plains1Select()
    {

        controlObj.DrawSTile = 9;

        PostDraw();
    }
    public void Plains2Select()
    {

        controlObj.DrawSTile = 10;

        PostDraw();
    }
    public void Plains3Select()
    {

        controlObj.DrawSTile = 11;

        PostDraw();
    }
    public void Rock1Select()
    {

        controlObj.DrawSTile = 12;

        PostDraw();
    }
    public void Rock2Select()
    {

        controlObj.DrawSTile = 13;

        PostDraw();
    }
    public void Rock3Select()
    {

        controlObj.DrawSTile = 14;

        PostDraw();
    }
    public void Grass1Select()
    {

        controlObj.DrawSTile = 15;

        PostDraw();
    }
    public void Grass2Select()
    {

        controlObj.DrawSTile = 16;

        PostDraw();
    }
    public void Grass3Select()
    {

        controlObj.DrawSTile = 17;

        PostDraw();
    }
    public void StoneWall1Select()
    {

        controlObj.DrawSTile = 18;

        PostDraw();
    }
    public void StoneWall2Select()
    {

        controlObj.DrawSTile = 19;

        PostDraw();
    }
    public void StoneWall3Select()
    {

        controlObj.DrawSTile = 20;

        PostDraw();
    }
    public void Bush1Select()
    {

        controlObj.DrawSTile = 21;

        PostDraw();
    }
    public void Bush2Select()
    {

        controlObj.DrawSTile = 22;

        PostDraw();
    }
    public void Bush3Select()
    {

        controlObj.DrawSTile = 23;

        PostDraw();
    }
    public void HealthBush1Select()
    {

        controlObj.DrawSTile = 24;

        PostDraw();
    }
    public void HealthBush2Select()
    {

        controlObj.DrawSTile = 25;

        PostDraw();
    }
    public void HealthBush3Select()
    {

        controlObj.DrawSTile = 26;

        PostDraw();
    }

    public void InvigoratingBush1Select()
    {

        controlObj.DrawSTile = 27;

        PostDraw();
    }
    public void InvigoratingBush2Select()
    {

        controlObj.DrawSTile = 28;

        PostDraw();
    }
    public void InvigoratingBush3Select()
    {

        controlObj.DrawSTile = 29;

        PostDraw();
    }

    public void FortifyingBush1Select()
    {

        controlObj.DrawSTile = 30;

        PostDraw();
    }
    public void FortifyingBush2Select()
    {

        controlObj.DrawSTile = 31;

        PostDraw();
    }
    public void FortifyingBush3Select()
    {

        controlObj.DrawSTile = 32;

        PostDraw();
    }

    public void ReflexBush1Select()
    {

        controlObj.DrawSTile = 33;

        PostDraw();
    }
    public void ReflexBush2Select()
    {

        controlObj.DrawSTile = 34;

        PostDraw();
    }
    public void ReflexBush3Select()
    {

        controlObj.DrawSTile = 35;

        PostDraw();
    }


    public void QuickSand1Select()
    {

        controlObj.DrawSTile = 36;

        PostDraw();
    }
    public void QuickSand2Select()
    {

        controlObj.DrawSTile = 37;

        PostDraw();
    }
    public void QuickSand3Select()
    {

        controlObj.DrawSTile = 38;

        PostDraw();
    }

    public void Mountain1Select()
    {

        controlObj.DrawSTile = 39;

        PostDraw();
    }
    public void Mountain2Select()
    {

        controlObj.DrawSTile = 40;

        PostDraw();
    }
    public void Mountain3Select()
    {

        controlObj.DrawSTile = 41;

        PostDraw();
    }

    public void Spike1Select()
    {

        controlObj.DrawSTile = 42;

        PostDraw();
    }
    public void Spike2Select()
    {

        controlObj.DrawSTile = 43;

        PostDraw();
    }
    public void Spike3Select()
    {

        controlObj.DrawSTile = 44;

        PostDraw();
    }

    public void PureWater1Select()
    {

        controlObj.DrawSTile = 45;

        PostDraw();
    }
    public void PureWater2Select()
    {

        controlObj.DrawSTile = 46;

        PostDraw();
    }
    public void PureWater3Select()
    {

        controlObj.DrawSTile = 47;

        PostDraw();
    }

    public void Shoal1Select()
    {

        controlObj.DrawSTile = 48;

        PostDraw();
    }
    public void Shoal2Select()
    {

        controlObj.DrawSTile = 49;

        PostDraw();
    }
    public void Shoal3Select()
    {

        controlObj.DrawSTile = 50;

        PostDraw();
    }

    public void Monument1Select()
    {

        controlObj.DrawSTile = 51;

        PostDraw();
    }
    public void Monument2Select()
    {

        controlObj.DrawSTile = 52;

        PostDraw();
    }
    public void Monument3Select()
    {

        controlObj.DrawSTile = 53;

        PostDraw();
    }

    void Start()
    {
        if (controlObj == null)
        {
            controlObj = this;
            DontDestroyOnLoad(controlObj);
        }
        else if (controlObj != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

        //Let the gameobject persist over the scenes logic - borrowed from stack overflow
        
        //Check if the control instance is null


    
    }

    [Serializable]
    public class SaveCharData
    {
        public string name;

        public int pW = 0; //primary weapon
        public int sW = 0; //secondary weapon

        public int cH = 0;//hat cosmetic
        public int cB = 0; //body cosmetic
    }

    [Serializable]
    public class SaveMapData
    {
        public string MapName;
        public int MapWidth;
        public int MapHeight;
        public IDsMix[][] TilesArrayID;

        public bool HighestHpWin; //default initialized
        public int HighestHpWinTurnLimit; //default initialized

        public bool MonumentWin; //default initialized
        public int MonumentTurnLimit; //default initialized

    }
}
