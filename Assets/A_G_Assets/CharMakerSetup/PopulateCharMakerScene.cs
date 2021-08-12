using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class PopulateCharMakerScene : MonoBehaviour
{

    static PopulateCharMakerScene Ml;

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

		Debug.Log("Loaded Save");
		//

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file =
				   File.Open(files[i], FileMode.Open);
		StaticDataMapMaker.SaveCharData data = (StaticDataMapMaker.SaveCharData)bf.Deserialize(file);
		file.Close();
		
		StaticDataMapMaker.controlObj.CharPassPW = data.pW;
		StaticDataMapMaker.controlObj.CharPassSW = data.sW;
		StaticDataMapMaker.controlObj.CharPassCH = data.cH;
		StaticDataMapMaker.controlObj.CharPassCB = data.cB;
		StaticDataMapMaker.controlObj.CharPassName = data.name;

		SceneManager.LoadScene("CharacterMakerMaker");
	}

	void Populate()
	{

		if (Directory.Exists(Application.persistentDataPath
						 + "/Chars/"))
		{
			// This path is a file


			files = Directory.GetFiles(Application.persistentDataPath
						 + "/Chars/");

			privTmp = new GameObject[files.GetLength(0)];

			for (int i = 0; i < files.GetLength(0); i++)
			{
				int tmpVar = i;
				privTmp[i] = new GameObject();
				privTmp[i] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[i].GetComponent<TMP_Text>().text = (files[i].Replace(Application.persistentDataPath + "/Chars/", "")).Replace(".dat", ""); ;
				privTmp[i].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });
			}
			if (files.GetLength(0) == 0)
			{
				privTmp = new GameObject[1];
				privTmp[0] = new GameObject();
				privTmp[0] = Instantiate(SettingsForAllTEXT, transform);
				privTmp[0].GetComponent<TMP_Text>().text = "create a Char before you try to edit a Char!";
			}



		}

		else
		{

			FileInfo fileInfo = new FileInfo(Application.persistentDataPath
					 + "/Chars/");

			fileInfo.Directory.Create(); //make not existing file path


		}
		// Create new instances of our prefab until we've created as many as we specified
		//		m_TextComponent = Instantiate(m_TextComponent, transform);

	}
}
