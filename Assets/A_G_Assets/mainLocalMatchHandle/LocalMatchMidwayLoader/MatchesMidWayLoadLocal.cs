using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchesMidWayLoadLocalMatch : MonoBehaviour
{
	static MatchesMidWayLoadLocalMatch Ml;

	public GameObject SettingsForAllTEXT;

    GameObject[] privTmp;

    string[] files;

    public int indexClicked;

    // Start is called before the first frame update
    void Start()
    {
		Ml = this;

		Populate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void LoadToEdit(int i)
	{
		Ml.indexClicked = i;

		StaticDataMapMaker.controlObj.LoadMapDatPath = files[i];

		Debug.Log("Loaded Save");
		//
		StaticDataMapMaker.controlObj.LoadMapSaveDat = true;

		//load "TilesArrayID" stats
		SceneManager.LoadScene("");
	}

	void Populate()
	{

		if (Directory.Exists(Application.persistentDataPath
						 + "/MapsLocalSave/"))
		{
			// This path is a file


			files = Directory.GetFiles(Application.persistentDataPath
						 + "/MapsLocalSave/");

			privTmp = new GameObject[files.GetLength(0)];

			for (int i = 0; i < files.GetLength(0); i++)
			{
				int tmpVar = i;
				privTmp[i] = new GameObject();
				privTmp[i] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[i].GetComponent<TMP_Text>().text = (files[i].Replace(Application.persistentDataPath
						 + "/MapsLocalSave/", "")).Replace(".dat", "");
				privTmp[i].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

			}

			if (files.GetLength(0) == 0)
			{
				privTmp = new GameObject[1];
				privTmp[0] = new GameObject();
				privTmp[0] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[0].GetComponent<TMP_Text>().text = "start a match before you try to load a mid-way-match!";
			}


		}

		else
		{

			FileInfo fileInfo = new FileInfo(Application.persistentDataPath
					 + "/MapsLocalSave/");

			fileInfo.Directory.Create(); //make not existing file path


		}
	}
	}
