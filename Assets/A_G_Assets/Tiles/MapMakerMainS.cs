using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMakerMainS : MonoBehaviour
{
    public GameObject MapSetupState;

    public GameObject MapMakerModeState;

    public void loadScene()
    {
        SceneManager.LoadScene("MapMaker_Scene");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
