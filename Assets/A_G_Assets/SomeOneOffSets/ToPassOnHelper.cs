using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToPassOnHelper : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
