using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MapMakerSetup : MonoBehaviour
{
    static MapMakerSetup thisSetup;

    public GameObject inputField;
    GameObject inputFieldc;
    
    TMP_InputField inputFieldN;
    TMP_InputField inputFieldX;
    TMP_InputField inputFieldY;

    string inputTextName = "NULL";
    int inputTextWidth = 10;
    int inputTextHeight = 10;

    public void SubmitAllData()
    {
       // Debug.Log(MapMakerSetup.thisSetup.inputTextName);
        //pass data to perm storage
        StaticDataMapMaker.controlObj.MapName = MapMakerSetup.thisSetup.inputTextName;
        StaticDataMapMaker.controlObj.MapWidth = MapMakerSetup.thisSetup.inputTextWidth;
        StaticDataMapMaker.controlObj.MapHeight = MapMakerSetup.thisSetup.inputTextHeight;

        DrawGrid.GridLines.UpdateDrawGridWidthBasedMap();
        DrawGrid.GridLines.UpdateDrawGridHeightBasedMap();

#if UNITY_EDITOR
        Debug.Log("MapMaker_Maker Loaded!");
#endif
        SceneManager.LoadScene("MapMaker_Maker");

    }

    public void InputMapNameString()
    {
#if UNITY_EDITOR
      //  Debug.Log(inputTextName);
#endif
        MapMakerSetup.thisSetup.inputTextName = inputFieldN.text;
    }
    public void InputMapWidthString()
    {
#if UNITY_EDITOR
      //  Debug.Log("");
#endif
        MapMakerSetup.thisSetup.inputTextWidth = int.Parse(inputFieldX.text);
    }
    public void InputMapHeightString()
    {
#if UNITY_EDITOR
      //  Debug.Log("");
#endif
        MapMakerSetup.thisSetup.inputTextHeight = int.Parse(inputFieldY.text);
    }

    // Start is called before the first frame update
    void Start()
    {
        thisSetup = this;
        
        inputFieldc = Instantiate(inputField, new Vector3(0, 0, 0), Quaternion.identity);

        Transform[] Ch = inputFieldc.GetComponentsInChildren<Transform>(); 
            
        foreach(Transform child in Ch)
        {
            if (child.gameObject.name == "MapX")
            {
                inputFieldX = child.gameObject.GetComponent< TMP_InputField>();
            }

            else if (child.gameObject.name == "MapY")
            {
                inputFieldY = child.gameObject.GetComponent<TMP_InputField>();
            }

            else if (child.gameObject.name == "MapName")
            {
                inputFieldN = child.gameObject.GetComponent<TMP_InputField>();
            }
        }
        //  //set buffer to selected input field
    }

    // Update is called once per frame
    void Update()
    {
        //meh, I can poll every frame... not like I am doing anything important
        InputMapNameString();
        InputMapWidthString();
        InputMapHeightString();

    }
}
