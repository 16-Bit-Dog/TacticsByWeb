using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FillPW : MonoBehaviour
{
    GameObject[] privTmp;

    public GameObject TextWithB;
    public GameObject ImageWithB;
    // Start is called before the first frame update

    void LoadToEdit(int i)
    {

        CharMakerMakerObj.co.Cdata.pW = i;

        CharMakerMakerObj.co.InfoText.text = PriWLookup.PWLO.StatsAndInfo[i];

        //        Debug.Log(CharMakerMakerObj.co.Cdata.pW);
    }

    void Start()
    {
        
        privTmp = new GameObject[PriWLookup.PWLO.Name.Count * 2];

        for (int i = 0; i < PriWLookup.PWLO.Name.Count; i++)
        {

            int tmpVar = i;

            privTmp[i * 2] = new GameObject();
            privTmp[i * 2 + 1] = new GameObject();

            privTmp[i * 2] = Instantiate(ImageWithB, transform);
            privTmp[i * 2].GetComponent<Image>().sprite = PriWLookup.PWLO.Image[i];
            privTmp[i * 2].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });


            privTmp[i * 2 + 1] = Instantiate(TextWithB, transform);
            privTmp[i * 2 + 1].GetComponent<TMP_Text>().text = PriWLookup.PWLO.Name[i];
            privTmp[i * 2 + 1].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
