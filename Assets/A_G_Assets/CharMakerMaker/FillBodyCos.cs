using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FillBodyCos : MonoBehaviour
{
    GameObject[] privTmp;

    public GameObject TextWithB;
    public GameObject ImageWithB;

    void LoadToEdit(int i)
    {

        CharMakerMakerObj.co.Cdata.cB = i;

        CharMakerMakerObj.co.InfoText.text = BodyLookUp.BLO.StatsAndInfo[i];

    }

    // Start is called before the first frame update
    void Start()
    {
        privTmp = new GameObject[BodyLookUp.BLO.Name.Count * 2];

        for (int i = 0; i < BodyLookUp.BLO.Name.Count; i++)
        {
            int tmpVar = i;

            privTmp[i * 2] = new GameObject();
            privTmp[i * 2 + 1] = new GameObject();

            privTmp[i * 2] = Instantiate(ImageWithB, transform);
            privTmp[i * 2].GetComponent<Image>().sprite = BodyLookUp.BLO.Image[i];
            privTmp[i * 2].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });


            privTmp[i * 2 + 1] = Instantiate(TextWithB, transform);
            privTmp[i * 2 + 1].GetComponent<TMP_Text>().text = BodyLookUp.BLO.Name[i];
            privTmp[i * 2 + 1].GetComponent<Button>().onClick.AddListener(() => { LoadToEdit(tmpVar); });

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
