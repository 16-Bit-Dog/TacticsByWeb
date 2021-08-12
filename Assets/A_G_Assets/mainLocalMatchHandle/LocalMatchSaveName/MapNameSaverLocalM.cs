using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapNameSaverLocalM : MonoBehaviour
{
    TMP_InputField inputFieldN;
    // Start is called before the first frame update
    void Start()
    {
        inputFieldN = this.GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        StaticDataMapMaker.controlObj.saveMapDatString = inputFieldN.text;
    }
}
