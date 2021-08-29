using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTextToID : MonoBehaviour
{
    TMP_Text inputFieldN;
    // Start is called before the first frame update
    void Start()
    {
        inputFieldN = this.GetComponent<TMP_Text>();   
    }

    // Update is called once per frame
    void Update()
    {
        inputFieldN.text = WebManage.WManage.id.ToString();
    }
}
