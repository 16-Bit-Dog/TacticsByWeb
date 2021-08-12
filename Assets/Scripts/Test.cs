using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject gm;
    public void press()
    {
        Instantiate(gm);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Camera.main.GetComponent<UnityTemplateProjects.SimpleCameraController>());
    }
}
