using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SendIDToFriendMatch : MonoBehaviour
{
    // Start is called before the first frame update
    TMP_InputField inputFieldN;

    public void FriendBatStart()
    {
        WebManage.WManage.TryToJoinFriend = true;
    }

    void Start()
    {
        WebManage.WManage.GiveMapDataFriendlies = true;

        inputFieldN = this.GetComponent<TMP_InputField>();

    }

    // Update is called once per frame
    void Update()
    {

        WebManage.WManage.TryToJoinFriendVar = UInt32.Parse(inputFieldN.text);

        if (WebManage.WManage.MoveToWaiting = true)
        {
            WebManage.WManage.MoveToWaiting = false;
            SceneManager.LoadScene("FriendBatWaitingRoom");
        }

    }
}
