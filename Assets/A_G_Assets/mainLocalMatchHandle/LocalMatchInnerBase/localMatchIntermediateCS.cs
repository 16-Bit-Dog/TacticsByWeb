using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class localMatchIntermediateCS : MonoBehaviour
{
    

    public void ChooseAndSetAnim(int PorSWeap, Animator animPlayer)

    {

        switch (PorSWeap)
        {
            case 1: //light sword anim
                animPlayer.SetTrigger("BasicLightSword");
               
                break;
            case 2: 
                animPlayer.SetTrigger("LongStabKatana");

                break;
            case 3:
                animPlayer.SetTrigger("ThrustingSword");

                break;
            case 4:
                animPlayer.SetTrigger("BasicHeavySword");

                break;
        }

    }
    
    public static localMatchIntermediateCS LMICS;
    //localMatchIntermediateCS.CharDat

    //localMatchIntermediateCS.LMICS.HighestHpWin
    public bool HighestHpWin = false;
    public int HighestHpWinTurnLimit = 30;

    //localMatchIntermediateCS.LMICS.MonumentWin
    public bool MonumentWin = false;
    public int MonumentTurnLimit = 30;

    public class CharDat : LocalMatchInnerBase.CharDat
    { //class deriving with C# sucks... I wish I was using c++
        /*
         public CharDat(LocalMatchInnerBase.CharDat oldCD)
        {
            PosX = oldCD.PosX;
            PosY = oldCD.PosY;

            team = oldCD.team; //based on overlap - // 0 is null, 1 is blue, 2 is red, 3 is yellow, 4 is green, 5 is purple

            Hp = oldCD.Hp;
            Atk = oldCD.Atk;
            Def = oldCD.Def;
            Mov = oldCD.Mov;
            PW = oldCD.PW; //Pri weapon id
            SW = oldCD.SW; //Secondary weapon id
            CH = oldCD.CH; //Cosmetic hat id
            CB = oldCD.CB; //Cosmetic body id

            name = oldCD.name; //
        }
*/
        public GameObject BodyObj = new GameObject();
        public GameObject CharObj = new GameObject();
        public GameObject HatObj = new GameObject();
        public GameObject WeaponObj = new GameObject();

        public GameObject HealthBar = new GameObject(); //2d planes for health
        public GameObject HealthBarBackLine = new GameObject();


        public Animator anim;

        public bool HasTurn = false;

        public bool MovedAlready = false;

        public int Ability1 = 0; //ready to be used
        public int Ability2 = 0; //ready to be used

        public int RngMinPri; //Range
        public int RngMaxPri;

        public int RngMinSec; //Range
        public int RngMaxSec;

        public int CurW = 1; //1 == primary
        //        public bool MovePhase;

        public bool ShowSecondary = false;

        public int AtkBuff = 0; //-1 is bad
        public int DefBuff = 0; //-1 is bad
        public int MovBuff = 0; //-1 is bad
        public int HpBuff = 0; //-1 is bad

        //clear by -1 per turn

        public int Atk2 = 0;
        public int Def2 = 0;

        public bool Dead = false; //this is not mutually inclusive with hp - since an ability later down the line do some tomfoolery seperate from HP and death status


    }

    public CharDat tmpCharDat;

    public List<CharDat> blueC = new List<CharDat>();
    public List<CharDat> redC = new List<CharDat>();
    public List<CharDat> yellowC = new List<CharDat>();
    public List<CharDat> greenC = new List<CharDat>();
    public List<CharDat> purpleC = new List<CharDat>();

    // Start is called before the first frame update

    public class MapMakerVarsDat
    {

        public GameObject Tile = new GameObject();
        public int TileId = 6;
        public int Overlap = 0; // 0 is null, 1 is blue, 2 is red, 3 is yellow, 4 is green, 5 is purple
        public CharDat CDat;
    }

    public MapMakerVarsDat[][] TilesArray;

    public string SaveMapName;


    [Serializable]
    public class InFlightCharData
    {

        public bool HasTurn = false;
        public bool MovedAlready = false;
        public int Ability1 = 0;
        public int Ability2 = 0;
        public int RngMinPri; 
        public int RngMaxPri;
        public int RngMinSec;
        public int RngMaxSec;
        public int CurW = 1; 
        public bool ShowSecondary = false;
        public int AtkBuff = 0; 
        public int DefBuff = 0; 
        public int MovBuff = 0; 
        public int HpBuff = 0; 
        public int Atk2 = 0;
        public int Def2 = 0;
        public bool Dead = false;
        public int PosX;
        public int PosY;
        public int team;
        public int Hp;
        public int Atk;
        public int Def;
        public int Mov;
        public int PW; 
        public int SW; 
        public int CH; 
        public int CB; 
        public string name;

        public float startRotationX; //eulerAngles
        public float startRotationY; //eulerAngles
        public float startRotationZ; //eulerAngles

    }

    [Serializable]
    public class MapMakerVarsSaveDat
    {

        public int TileId;
        public int CDatX = -1; // -1 is null char
        public int CDatY = -1; // -1 is null char

    }

    [Serializable]
    public class LocalMatchSaveMapData
    {

        ////LMICS vars
        public string SaveMapName; //LCIMS save map name

        public bool HighestHpWin = false;
        public int HighestHpWinTurnLimit = 30;
        public bool MonumentWin = false;
        public int MonumentTurnLimit = 30;

        //store tiles -- with some indiation of char data link
        //store char dat seperately?!?
        public List<InFlightCharData> BlueChar = new List<InFlightCharData>();
        public List<InFlightCharData> RedChar = new List<InFlightCharData>();
        public List<InFlightCharData> GreenChar = new List<InFlightCharData>();
        public List<InFlightCharData> YellowChar = new List<InFlightCharData>();
        public List<InFlightCharData> PurpleChar = new List<InFlightCharData>();
        //compile into dictionary of all chars using a tuple as key for (x,y) when loading to set char in positions

        public MapMakerVarsSaveDat[][] TilesArray; //char pos is where you fetch char

        ////

        //reg vars from local match start
        public int Winner = 0;
        public bool CalcMoveDone = true;
        public int MovePhase = 0;
        public int CXPos = 0;
        public int CYPos = 0;
        public bool currentlyFighting = false;
        public int currentTurn = 0;
        public int CurrentTeamTurn = 0;
        public int TeamCount = 0;
        public int MonumentValBlue = 0;
        public int MonumentValRed = 0;
        public int MonumentValYellow = 0;
        public int MonumentValGreen = 0;
        public int MonumentValPurple = 0;
        public bool PermaHoverInfoTextOff = false;
        public string HoverInfoTMP = "";
        public bool HideTileInfo;
        public bool attackPrep = false;

        //need to convert vars
        public Tuple<int, int> HoverTupleStore; //x y pos for tuples and list
        public Tuple<int, int> SelectedAttackTarget;

        public List< Tuple<int, int> > TmpAbilityTiles = new List<Tuple<int, int>>(); //xy positions of tile to load into dictionary
        public List< Tuple<int, int> > TmpMoveTiles = new List<Tuple<int, int>>();
        public List< Tuple<int, int> > TmpAtkTiles = new List<Tuple<int, int>>();

        public int SelectedCharX; // X position
        public int SelectedCharY; // Y position

        // // is active state bools for game menu stuff
        public bool IsHoverInfoActive;
        public bool IsHoverInfoCloseButtonActive;
        public bool IsActionMenuObjActive;
        public bool IsCloseActionMenuObjActive;
        public bool IsCloseActionAtkObjActive;
        public bool IsActionAbilityInfoObjActive;
        public bool IsCloseActionAbilityObjActive;
        public bool IsAttack1Active;
        public bool AIsttack2Active;
        public bool IsAbility1Active;
        public bool IsAbility2Active;
        public bool IsMoveActive;
        public bool IsTileInfoObjActive;
        public bool IsTileInfoToggleActive;
        public bool IsAttackPrepScreenAttackerActive;
        public bool IsAttackPrepScreenDefenderActive;
        public bool IsStartToAttackButtonActive;
        public bool IsCloseActionRotateObjBackButtonActive;
        public bool IsWinConditionObjActive;
        //anim vars?

    }

    void Awake()
    {
        
        if (LMICS == null)
        {
            LMICS = this;
        }
        else if (LMICS != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

        //Let the gameobject persist over the scenes logic - borrowed from stack overflow
        DontDestroyOnLoad(LMICS);

        LMICS.tmpCharDat = new CharDat();

        LMICS.SaveMapName = StaticDataMapMaker.controlObj.saveMapDatString;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
