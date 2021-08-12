using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatLookup : MonoBehaviour
{
    public static HatLookup HLO;
 
    public List<GameObject> Hat; //I can fill with empty game objects rather than models for place holding - I may not make 2 sets of models
    public List<Sprite> Image;
    public List<string> Name; //must be the same size as the SecWeapons
    public List<string> StatsAndInfo; //fun hat info

    // Start is called before the first frame update
    void Start()
    {
        if (HLO == null)
        {
            HLO = this;
        }
        else if (HLO != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(HLO);

        for (int i = 0; i < HLO.StatsAndInfo.Count; i++)
        {

            HLO.StatsAndInfo[i] = HLO.StatsAndInfo[i].Replace("\\n", "\n");

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
