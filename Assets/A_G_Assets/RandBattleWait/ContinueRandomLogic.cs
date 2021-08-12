using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueRandomLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (StaticDataMapMaker.controlObj.LoadMapDatPath == (Application.persistentDataPath + "/RR1.txt"))
        {
            SceneManager.LoadScene("RandomOnlinePregame");
        }
    }
}
