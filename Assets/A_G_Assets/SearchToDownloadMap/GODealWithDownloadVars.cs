using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Reflection;
using System;
using System.IO;

public class GODealWithDownloadVars : MonoBehaviour
{
    int indexClicked = 0;
    
    static GODealWithDownloadVars ML;

    public GameObject ScrollViewMaps;

    public GameObject SearchStringOBJ;
    TMP_InputField SearchString;

    public GameObject PlayerCountSearchOBJ;
    TMP_InputField PlayerCountSearch;

    public GameObject FilterAuthIDOBJ;
    Toggle FilterAuthID;

    public GameObject FilterMapIDOBJ;
    Toggle FilterMapID;

    public GameObject FilterNameSearchOBJ;
    Toggle FilterNameSearch;


    List<string> files;
	GameObject[] privTmp = new GameObject[0];
	public GameObject SettingsForAllTEXT;


    // Start is called before the first frame update
    void Start()
    {
        ML = this;

        SearchString = SearchStringOBJ.GetComponent<TMP_InputField>();

        PlayerCountSearch = PlayerCountSearchOBJ.GetComponent<TMP_InputField>();        
    
        FilterAuthID = FilterAuthIDOBJ.GetComponent<Toggle>();
    
        FilterMapID = FilterMapIDOBJ.GetComponent<Toggle>();

        FilterNameSearch = FilterNameSearchOBJ.GetComponent<Toggle>();
    }

    public void ClickedFilterAuthID()
    {
        ML.FilterAuthID.isOn = true; 
        ML.FilterMapID.isOn = false;
        ML.FilterNameSearch.isOn = false;

        WebManage.WManage.SearchType = 2;
    }
    public void ClockedFilterMapID()
    {
        ML.FilterAuthID.isOn = false; 
        ML.FilterMapID.isOn = true;
        ML.FilterNameSearch.isOn = false;

        WebManage.WManage.SearchType = 1;
    }
    public void ClockedFilterNameSearch()
    {
        ML.FilterAuthID.isOn = false; 
        ML.FilterMapID.isOn = false;
        ML.FilterNameSearch.isOn = true;

        WebManage.WManage.SearchType = 0;
    }

    IEnumerator ToUpdateScrollView()
    {
        while(WebManage.WManage.searchForDownloadMap == true)
        {
            yield return null;
        }
        
        ML.files = JsonConvert.DeserializeObject<List<string>>(WebManage.WManage.JsonReceiveS.s);
		
        ML.RePopulate();
        
        //
        //TODO: UPDATE SCROLL VIEW WITH DATA
    }

    public void GOGODownload()
    {
        WebManage.WManage.SearchS = ML.PlayerCountSearch.text;
        WebManage.WManage.teamsCountForM = Int32.Parse(ML.PlayerCountSearch.text);
        WebManage.WManage.searchForDownloadMap = true;

        IEnumerator TU = ML.ToUpdateScrollView();
    
        StartCoroutine(TU);
    }

    public void RePopulate()
    {
        for(int i = 0; i < privTmp.Length; i++)
        {
            Destroy(privTmp[i]);
        }
        Array.Clear(privTmp, 0, privTmp.Length);

        ML.Populate();
    }

    void Populate()
	{
		privTmp = new GameObject[files.Count];

		for (int i = 0; i < files.Count; i++)
		{
			int tmpVar = i;
			privTmp[i] = new GameObject();
			privTmp[i] = Instantiate(SettingsForAllTEXT, ScrollViewMaps.transform);
			privTmp[i].GetComponent<TMP_Text>().text = files[i];
			privTmp[i].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

		}

		if (files.Count == 0)
		{
			privTmp = new GameObject[1];
			privTmp[0] = new GameObject();
			privTmp[0] = Instantiate(SettingsForAllTEXT, ScrollViewMaps.transform);
			privTmp[0].GetComponent<TMP_Text>().text = "start a match before you try to load a match!";
          //  privTmp[0].transform.SetParent(ScrollViewMaps);
        }

	}

	void LoadToEdit(int i)
	{
        if(WebManage.WManage.id != 0)
        {
		
        
        ML.indexClicked = i;

		//

		StaticDataMapMaker.controlObj.LoadMapDatPath = i.ToString();
		
		WebManage.WManage.JsonReceiveS.s = "";
		//StaticDataMapMaker.controlObj.LoadMapDat = true;
		
		WebManage.WManage.GetMapDataForDownload = true;

		IEnumerator lw = LoadWait();
		StartCoroutine(lw);
		

        }
	} //StaticDataMapMaker.controlObj.saveMapDatString <- map name

	IEnumerator LoadWait()
	{
		while(WebManage.WManage.GetMapDataForDownload == true){yield return null;}

		Debug.Log("Loaded Map");

		StaticDataMapMaker.controlObj.LoadMapSaveDat = true;
		
		WebManage.WManage.MatchType = 1;

        StaticDataMapMaker.controlObj.LoadDownloadShowOff();
	}

    public void SaveFileFull()
    {
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath
            + "/Maps/");

        if (!fileInfo.Directory.Exists) fileInfo.Directory.Create(); //make not existing file path
        
        File.WriteAllText(Application.persistentDataPath
            + "/Maps/" + StaticDataMapMaker.controlObj.saveMapDatString + ".dat", WebManage.WManage.JsonReceiveS.s);


        StaticDataMapMaker.controlObj.LoadMain();
    }

/*
    public string SearchS = "";
    public int SearchType = 0;
    public int teamsCountForM = 2;
*/
    // Update is called once per frame
    void Update()
    {
        
    }
}
