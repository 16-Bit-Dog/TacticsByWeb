using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapLocalMatchLoad : MonoBehaviour
{//remember to instantiate new objects such that it follows the row logic

	static MapLocalMatchLoad Ml;

	public GameObject SettingsForAllTEXT;

	GameObject[] privTmp;

	string[] files;

	public int indexClicked;

	void Start()
	{
		Ml = this;
		Populate();
	}

	void Update()
	{
		//Debug.Log(files[1]);
	}

	void LoadToEdit(int i)
	{
		Ml.indexClicked = i;

		//

		StaticDataMapMaker.controlObj.LoadMapDatPath = files[i];
		StaticDataMapMaker.controlObj.LoadMapDat = true;
		Debug.Log("Loaded Save");
		//


		//load "TilesArrayID" stats
		SceneManager.LoadScene("LocalMatchInner");
	}

	void Populate()
	{

		if (Directory.Exists(Application.persistentDataPath
						 + "/Maps/"))
		{
			// This path is a file


			files = Directory.GetFiles(Application.persistentDataPath
						 + "/Maps/");

			privTmp = new GameObject[files.GetLength(0)];

			for (int i = 0; i < files.GetLength(0); i++)
			{
				int tmpVar = i;
				privTmp[i] = new GameObject();
				privTmp[i] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[i].GetComponent<TMP_Text>().text = (files[i].Replace(Application.persistentDataPath
						 + "/Maps/", "")).Replace(".dat", "");
				privTmp[i].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

			}

			if(files.GetLength(0) == 0)
            {
				privTmp = new GameObject[1];
				privTmp[0] = new GameObject();
				privTmp[0] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[0].GetComponent<TMP_Text>().text = "create a map before you try to edit a map!";
			}


		}

		else
		{

			FileInfo fileInfo = new FileInfo(Application.persistentDataPath
					 + "/Maps/");

			fileInfo.Directory.Create(); //make not existing file path


		}
		// Create new instances of our prefab until we've created as many as we specified
		//		m_TextComponent = Instantiate(m_TextComponent, transform);

	}
}