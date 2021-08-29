using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using TMPro;
using Newtonsoft.Json;

public class DeleteUploadMapSelectLoad : MonoBehaviour
{

	static DeleteUploadMapSelectLoad Ml;

	public GameObject SettingsForAllTEXT;

	GameObject[] privTmp = new GameObject[0];

	List<string> files;

	public int indexClicked;

    IEnumerator WaitToDeleteUploadMap()
    {
        while(WebManage.WManage.DeleteUploadMapData){yield return null;}

        StaticDataMapMaker.controlObj.LoadMain();
    }

    public void DeleteUploadedMap()
    {
        WebManage.WManage.DeleteUploadMapData = true;

        IEnumerator tmpE = WaitToDeleteUploadMap();

        StartCoroutine(tmpE);
        //StaticDataMapMaker.controlObj.LoadMapDatPath // <-- delete
    }

	void Start()
	{

		Ml = this;
		
		WebManage.WManage.fetchMapsInProgressUpload = true;

	}

	void Update()
	{
		if(WebManage.WManage.fetchMapsInProgressUpload == false && privTmp.Length == 0)
		{
			files = JsonConvert.DeserializeObject<List<string>>(WebManage.WManage.JsonReceiveS.s);
			Populate();
		}
		//Debug.Log(files[1]);
	}

	IEnumerator LoadWait()
	{
		while(WebManage.WManage.SendBackAndGetMapInProgress == true){yield return null;}

		Debug.Log("Loaded Map");

		StaticDataMapMaker.controlObj.LoadMapSaveDat = true;
		
		WebManage.WManage.MatchType = 1;

        StaticDataMapMaker.controlObj.LoadShowOffToRemoveUploadedMap();
	}

	void LoadToEdit(int i)
	{
        if(WebManage.WManage.id != 0)
        {
		
        
        Ml.indexClicked = i;

		//

		StaticDataMapMaker.controlObj.LoadMapDatPath = i.ToString();
		
		WebManage.WManage.JsonReceiveS.s = "";
		//StaticDataMapMaker.controlObj.LoadMapDat = true;
		
		WebManage.WManage.SendBackAndGetMapInProgressUpload = true;

		IEnumerator lw = LoadWait();
		StartCoroutine(lw);
		

        }
	} //StaticDataMapMaker.controlObj.saveMapDatString <- map name

void Populate()
	{
	

			privTmp = new GameObject[files.Count];

			for (int i = 0; i < files.Count; i++)
			{
				int tmpVar = i;
				privTmp[i] = new GameObject();
				privTmp[i] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[i].GetComponent<TMP_Text>().text = files[i];
				privTmp[i].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

			}

			if (files.Count == 0)
			{
				privTmp = new GameObject[1];
				privTmp[0] = new GameObject();
				privTmp[0] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[0].GetComponent<TMP_Text>().text = "start a match before you try to load a match!";
			}

	}
}

