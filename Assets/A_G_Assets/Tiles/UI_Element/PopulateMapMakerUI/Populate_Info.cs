using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Populate_Info : MonoBehaviour
{
    //   Populate_Info.Name.GetComponent<TMP_Text>().text = "";

    static public Populate_Info PI;
    public TMP_Text Info; //take in TMP_Text vars


    // Start is called before the first frame update
    void Start()
    {
        if (PI == null)
        {
            PI = this;
        }
        else if (PI != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(PI);
        }

        //Let the gameobject persist over the scenes logic - borrowed from stack overflow
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
