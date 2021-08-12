using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FillSW : MonoBehaviour
{
    GameObject[] privTmp;

    public GameObject TextWithB;
    public GameObject ImageWithB;


    void LoadToEdit(int i)
    {

        CharMakerMakerObj.co.Cdata.sW = i;

        CharMakerMakerObj.co.InfoText.text = SecWLookup.SWLO.StatsAndInfo[i];

//        Debug.Log(CharMakerMakerObj.co.Cdata.sW);
    }

    // Start is called before the first frame update
    void Start()
    {

        privTmp = new GameObject[SecWLookup.SWLO.Name.Count*2];

        for (int i = 0; i < SecWLookup.SWLO.Name.Count; i++)
        {
            int tmpVar = i;

            privTmp[i * 2] = new GameObject();
            privTmp[i * 2 + 1] = new GameObject();

            privTmp[i * 2] = Instantiate(ImageWithB, transform);
            privTmp[i * 2].GetComponent<Image>().sprite = SecWLookup.SWLO.Image[i];
            privTmp[i * 2].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });


            privTmp[i * 2 + 1] = Instantiate(TextWithB, transform);
            privTmp[i * 2 + 1].GetComponent<TMP_Text>().text = SecWLookup.SWLO.Name[i];
            privTmp[i * 2 + 1].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
