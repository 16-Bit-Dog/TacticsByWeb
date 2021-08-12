using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FillHatCos : MonoBehaviour
{
    GameObject[] privTmp;

    public GameObject TextWithB;
    public GameObject ImageWithB;

    void LoadToEdit(int i)
    {
        Destroy(CharMakerMakerObj.co.hatObj);

        CharMakerMakerObj.co.Cdata.cH = i;
        CharMakerMakerObj.co.InfoText.text = HatLookup.HLO.StatsAndInfo[i];
        CharMakerMakerObj.co.hatObj = Instantiate(HatLookup.HLO.Hat[i], new Vector3(0,0,0), Quaternion.identity); //new Vector3(-4, 4.5f, 0)

    }

    // Start is called before the first frame update
    void Start()
    {
        privTmp = new GameObject[HatLookup.HLO.Name.Count * 2];

        for (int i = 0; i < HatLookup.HLO.Name.Count; i++)
        {
            int tmpVar = i;

            privTmp[i * 2] = new GameObject();
            privTmp[i * 2 + 1] = new GameObject();

            privTmp[i * 2] = Instantiate(ImageWithB, transform);
            privTmp[i * 2].GetComponent<Image>().sprite = HatLookup.HLO.Image[i];
            privTmp[i * 2].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });


            privTmp[i * 2 + 1] = Instantiate(TextWithB, transform);
            privTmp[i * 2 + 1].GetComponent<TMP_Text>().text = HatLookup.HLO.Name[i];
            privTmp[i * 2 + 1].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
