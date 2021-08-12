using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MapMakerVars
{
    public GameObject Tile = new GameObject();
    public int TileId = 6;
    public int Overlap = 0; // 0 is null, 1 is blue, 2 is red, 3 is yellow, 4 is green, 5 is purple
    public GameObject CharObj = new GameObject();
}

[Serializable]
public class IDsMix {

    public int TileId = 6;
    public int Overlap = 0;

}

public class MapMakerMakerScript : MonoBehaviour
{
    public static MapMakerMakerScript MMSS;

    public GameObject[] TurnOffForWinConditionsVar;
    public GameObject[] TurnOnForWinConditionsVar;

    public GameObject ScrollViewTileView;

    public GameObject InfoScreen;

    private UnityTemplateProjects.SimpleCameraController.CameraState CamS; 

    public void DisAndReableScrollViewTileToggle()
    {
        if (ScrollViewTileView.active)
        {
            ScrollViewTileView.SetActive(false);
        }
        else
        {
            ScrollViewTileView.SetActive(true);
        }

    }
    public void DisAndReableInfoScreenToggle()
    {
        if (InfoScreen.active)
        {
            InfoScreen.SetActive(false);
        }
        else
        {
            InfoScreen.SetActive(true);
        }

    }

    public MapMakerVars[][] TilesArray;
    // Start is called before the first frame update

    public TMP_Text TileInfoText;

    bool ChangeWinConditions = false;

    public TMP_InputField HpTurnInputF;
    bool HighestHpWin = false;
    int HighestHpWinTurnLimit = 30;

    public TMP_InputField MonumentTurnInputF;
    bool MonumentWin = false;
    int MonumentTurnLimit = 30;


    public Toggle HpWinToggleButton;
    public Toggle MonumentWinToggleButton;

    public void ToggleHighestHpWin()
    {

        MMSS.HighestHpWin = !MMSS.HighestHpWin;
   
    }

    public void ToggleMonumentWin()
    {

        MMSS.MonumentWin = !MMSS.MonumentWin;

    }

    public void ChangeWinConditionsToggle()
    {

        MMSS.ChangeWinConditions = !MMSS.ChangeWinConditions;

        for (int i = 0; i < TurnOffForWinConditionsVar.GetLength(0); i++)
        {

            MMSS.TurnOffForWinConditionsVar[i].SetActive(!TurnOffForWinConditionsVar[i].activeSelf);

        }

        for (int i = 0; i < TurnOnForWinConditionsVar.GetLength(0); i++)
        {

            MMSS.TurnOnForWinConditionsVar[i].SetActive(!TurnOnForWinConditionsVar[i].activeSelf);

        }


    }

    void InstantiateAllTilesDefault()
    {
        for (int i = 0; i < TilesArray.GetLength(0); i++)
        {
            for (int ii = 0; ii < TilesArray[0].GetLength(0); ii++)
            {
                TilesArray[i][ii].Tile = Instantiate(TileLookUp.TLU.Tiles[6].Obj, new Vector3(4 * ii,-20, 4 * i), Quaternion.identity);
            }
        }
    }

    void ChangeTile(int y, int x, int TileID) //[y][x] is tile organization - StaticDataMapMaker.controlObj.DrawSTile
    {
        if (TileID > 5 && (TileLookUp.TLU.Tiles[TileID].MoveCost != 100 || TileLookUp.TLU.Tiles[TileID].MoveCost != 99) )
        {
            Destroy(TilesArray[y][x].Tile);
            TilesArray[y][x].Tile = Instantiate(
            TileLookUp.TLU.Tiles[TileID].Obj,
            new Vector3(4 * x, -20, 4 * y),
            Quaternion.identity);
            TilesArray[y][x].TileId = TileID;
        }
        
        else if(TileID > 5 && (TileLookUp.TLU.Tiles[TileID].MoveCost == 100 || TileLookUp.TLU.Tiles[TileID].MoveCost == 99) && TilesArray[y][x].Overlap == 0)
        {

            Destroy(TilesArray[y][x].Tile);
            TilesArray[y][x].Tile = Instantiate(
            TileLookUp.TLU.Tiles[TileID].Obj,
            new Vector3(4 * x, -20, 4 * y),
            Quaternion.identity);
            TilesArray[y][x].TileId = TileID;

        }

        else if(TileID < 6 && TileLookUp.TLU.Tiles[TilesArray[y][x].TileId].MoveCost != 100)
        {
            Destroy(TilesArray[y][x].CharObj);
            
            TilesArray[y][x].CharObj = Instantiate( //clear char makes a empty prefab to fill with to not keep inconsistant
            TileLookUp.TLU.Tiles[TileID].Obj,
            new Vector3(4 * x, -17, 4 * y),
            Quaternion.identity);
            TilesArray[y][x].Overlap = TileID;
        }
    }


    public void SaveMap()
    {

        var dicCheck = new Dictionary<int, int>();

        for (int i = 0; i < TilesArray.GetLength(0); i++)
        {
            for (int ii = 0; ii < TilesArray[0].GetLength(0); ii++)
            {
                dicCheck[TilesArray[i][ii].Overlap] = 0; 
            }
        }

        if (dicCheck.Count>2) //0, and 2 other overlaps
        { //at least 2 diffrent overlaps which are not 0

            FileInfo fileInfo = new FileInfo(Application.persistentDataPath
                         + "/Maps/");

            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create(); //make not existing file path

            StaticDataMapMaker.SaveMapData data = new StaticDataMapMaker.SaveMapData();

            data.MonumentTurnLimit = MMSS.MonumentTurnLimit;
            data.MonumentWin = MonumentWinToggleButton.isOn;
            data.HighestHpWinTurnLimit = MMSS.HighestHpWinTurnLimit;
            data.HighestHpWin = HpWinToggleButton.isOn;

            data.MapName = StaticDataMapMaker.controlObj.MapName;
            data.MapWidth = StaticDataMapMaker.controlObj.MapWidth;
            data.MapHeight = StaticDataMapMaker.controlObj.MapHeight;

            data.TilesArrayID = new IDsMix[StaticDataMapMaker.controlObj.MapHeight][];
            for (int i = 0; i < TilesArray.GetLength(0); i++)
            {
                data.TilesArrayID[i] = new IDsMix[StaticDataMapMaker.controlObj.MapWidth];

                for (int ii = 0; ii < TilesArray[0].GetLength(0); ii++)
                {
                    data.TilesArrayID[i][ii] = new IDsMix();
                    data.TilesArrayID[i][ii].Overlap = TilesArray[i][ii].Overlap;
                    data.TilesArrayID[i][ii].TileId = TilesArray[i][ii].TileId;
                }
            }

            File.WriteAllText(Application.persistentDataPath
                         + "/Maps/" + StaticDataMapMaker.controlObj.MapName + ".dat", JsonConvert.SerializeObject(data));


            SceneManager.LoadScene("Main");
        }
    }

    void LoadMapToEdit()
    {
        if (StaticDataMapMaker.controlObj.LoadMapDat == true)
        {
            Debug.Log("Save Load Part 2");
            StaticDataMapMaker.controlObj.LoadMapDat = false;

            StaticDataMapMaker.SaveMapData data = JsonConvert.DeserializeObject<StaticDataMapMaker.SaveMapData>(File.ReadAllText(StaticDataMapMaker.controlObj.LoadMapDatPath));

            StaticDataMapMaker.controlObj.MapName = data.MapName;
            StaticDataMapMaker.controlObj.MapWidth = data.MapWidth;
            StaticDataMapMaker.controlObj.MapHeight = data.MapHeight;

            //if (data.MonumentTurnLimit != null) //versioning
            //{


            HighestHpWin = data.HighestHpWin;
            HighestHpWinTurnLimit = data.HighestHpWinTurnLimit;
            MonumentWin = data.MonumentWin;
            MonumentTurnLimit = data.MonumentTurnLimit;

            HpWinToggleButton.isOn = data.HighestHpWin;
            MonumentWinToggleButton.isOn = data.MonumentWin;

            //}
            /*else
            {
                MMSS.MonumentTurnLimit = 30;
                MMSS.MonumentWin = false;
                MMSS.HighestHpWinTurnLimit = 30;
                MMSS.HighestHpWin = false;
            }
            */

            //Debug.Log(data.MapName);
            //Debug.Log(data.MapWidth);
            //Debug.Log(data.MapHeight);
            //Debug.Log(data.TilesArrayID[0][0].Overlap);

            TilesArray = new MapMakerVars[StaticDataMapMaker.controlObj.MapHeight][];
            for (int i = 0; i < TilesArray.GetLength(0); i++)
            {
                TilesArray[i] = new MapMakerVars[StaticDataMapMaker.controlObj.MapWidth];
                for (int ii = 0; ii < TilesArray[i].GetLength(0); ii++)
                {
                    TilesArray[i][ii] = new MapMakerVars();
                    TilesArray[i][ii].Overlap = data.TilesArrayID[i][ii].Overlap;
                    TilesArray[i][ii].TileId = data.TilesArrayID[i][ii].TileId;
                    TilesArray[i][ii].Tile = new GameObject();
                    TilesArray[i][ii].CharObj = new GameObject();
                    TilesArray[i][ii].Tile = Instantiate(TileLookUp.TLU.Tiles[TilesArray[i][ii].TileId].Obj, new Vector3(4 * ii, -20, 4 * i), Quaternion.identity);
                    TilesArray[i][ii].CharObj = Instantiate(TileLookUp.TLU.Tiles[TilesArray[i][ii].Overlap].Obj, new Vector3(4 * ii, -17, 4 * i), Quaternion.identity);
                
                }

            }
            DrawGrid.GridLines.UpdateDrawGridWidthBasedMap();
            DrawGrid.GridLines.UpdateDrawGridHeightBasedMap();
        }

        else
        {

            TilesArray = new MapMakerVars[StaticDataMapMaker.controlObj.MapHeight][];
            for (int i = 0; i < TilesArray.GetLength(0); i++)
            {
                TilesArray[i] = new MapMakerVars[StaticDataMapMaker.controlObj.MapWidth];
                for (int ii = 0; ii < TilesArray[i].GetLength(0); ii++)
                {
                    TilesArray[i][ii] = new MapMakerVars();
                    TilesArray[i][ii].Tile = new GameObject();
                    TilesArray[i][ii].CharObj = new GameObject();
                    TilesArray[i][ii].Overlap = 0;
                    TilesArray[i][ii].TileId = 6;
                }
            }

            InstantiateAllTilesDefault();

        }

    }

    void Start()
    {
        MMSS = this;

        LightRayToCoords.LRC.UIDEPTH = -10;

        LoadMapToEdit();

        DrawGrid.GridLines.UpdateGridDraw();

        Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().mouseCamSlide = true;

        CamS = Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState;

        MMSS.MonumentTurnInputF.text = "" + (MMSS.MonumentTurnLimit);
        MMSS.HpTurnInputF.text = "" + (MMSS.HighestHpWinTurnLimit);

    }

    // Update is called once per frame
    void Update()
    {
        
        //Debug.Log(StaticDataMapMaker.controlObj.MapName);
        if (CamS.x < 0)
        {

            CamS.x = 1;

        }

        if (CamS.x > 4* TilesArray[0].GetLength(0))
        {

            CamS.x = 4 * TilesArray[0].GetLength(0)-1;

        }


        if (CamS.z < 0)
        {

            CamS.z = 1;

        }

        if (CamS.z > 4 * TilesArray.GetLength(0))
        {

            CamS.z = 4 * TilesArray.GetLength(0)-1;

        }
        

        if (Input.GetMouseButton(0) && MMSS.ChangeWinConditions == false)
        {

            Vector3 tmpVec3 = LightRayToCoords.LRC.GetRayCamMouseXYZ();

            int CXPos = Convert.ToInt32((Math.Abs(tmpVec3.x / 4)));
            int CYPos = Convert.ToInt32((Math.Abs(tmpVec3.z / 4)));

            if (CXPos < 0 || CXPos > TilesArray[0].GetLength(0))
            {
                CXPos = 0;
            }
            if (CYPos < 0 || CYPos > TilesArray.GetLength(0))
            {
                CYPos = 0;
            }

            if (tmpVec3 != new Vector3(-10000, -10000, -10000))
            {
#if UNITY_EDITOR

#endif

                ChangeTile(CYPos, CXPos, StaticDataMapMaker.controlObj.DrawSTile);

                //CHANGE tile in grid BASED ON [floor(.y)%4][floor(.x)%4] = Change tile with Tile SelectedPOSITION OF X Y ratio gotten and you know all 4 for x and y (round down remainder 4) is 1 tile
            }
        }
        else if(MMSS.ChangeWinConditions == true)
        {

            MMSS.MonumentTurnLimit = int.Parse(MMSS.MonumentTurnInputF.text);
            MMSS.HighestHpWinTurnLimit = int.Parse(MMSS.HpTurnInputF.text);

        }

        //need to check if cam is outside bounds - Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().x;
    }
}
