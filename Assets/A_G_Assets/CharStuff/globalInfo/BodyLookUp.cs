using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyLookUp : MonoBehaviour
{
    public static BodyLookUp BLO;

    public List<GameObject> Body; //I can fill with empty game objects rather than models for place holding - I may not make 2 sets of models
    public List<Sprite> Image;
    public List<string> Name; //must be the same size as the SecWeapons
    public List<string> StatsAndInfo; //fun hat info

    // Start is called before the first frame update
    void Start()
    {
        if (BLO == null)
        {
            BLO = this;
        }
        else if (BLO != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(BLO);
      
        for (int i = 0; i < BLO.StatsAndInfo.Count; i++)
        {

            BLO.StatsAndInfo[i] = BLO.StatsAndInfo[i].Replace("\\n", "\n");

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
