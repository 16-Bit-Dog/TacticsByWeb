using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscTileObj : MonoBehaviour
{
    public static MiscTileObj MTO;
    
    public GameObject SelectTile;

    public GameObject AttackTile;

    public GameObject AbilityTile;
    // Start is called before the first frame update
    void Start()//I realized I did other script setups wrong... I will need to fix the arbitrary "thing = this;" and lock it to the if's so I don't override
    {
        if (MTO == null)
        {
            MTO = this;
        }
        else if (MTO != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

        //Let the gameobject persist over the scenes logic - borrowed from stack overflow
        DontDestroyOnLoad(MTO);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
