using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class setIDToText : MonoBehaviour
{
    TMP_Text inputFieldN;
    // Start is called before the first frame update

        // Start is called before the first frame update

    void Start()
    {
        WebManage.WManage.GiveMapDataFriendlies = true;

        inputFieldN = this.GetComponent<TMP_Text>();

        inputFieldN.text = WebManage.WManage.id.ToString();   
    }

    // Update is called once per frame
    void Update()
    {
        if (WebManage.WManage.FoundMatch)
        {
            SceneManager.LoadScene("FriendBatPreGame");
        }
    }
}
