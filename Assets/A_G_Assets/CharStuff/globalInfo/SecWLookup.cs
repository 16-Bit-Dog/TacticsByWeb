using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecWLookup : MonoBehaviour
{
    public static SecWLookup SWLO;
    //
    //these are worse spec'ed primarys

    public List<GameObject> SecWeapons; //I can fill with empty game objects rather than models for place holding - I may not make 2 sets of models
    public List<string> Name; //must be the same size as the SecWeapons
    public List<Sprite> Image;

    public List<int> Atk; //must be the same size as the SecWeapons
    public List<int> Def; //must be the same size as the SecWeapons
    public List<int> MinRng; //must be the same size as the SecWeapons
    public List<int> MaxRng; //must be the same size as the SecWeapons
    public List<int> MainAnimNum; //must be the same size as the SecWeapons

    public List<string> ItemBlurb;


    public List<string> StatsAndInfo; //must be the same size as the SecWeapons

    public List<int> AbilityNumber;

    // Start is called before the first frame update
    void Awake(){ }

    void Start()
    {
        if (SWLO == null)
        {
            SWLO = this;
        }
        else if (SWLO != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(SWLO);

        for(int i = 0; i < SWLO.StatsAndInfo.Count; i++)
        {
            SWLO.StatsAndInfo[i] = "-Name: " + SWLO.Name[i] + "\n-Atk: " + SWLO.Atk[i] + "\n-Def: " + SWLO.Def[i] + "\n-Range: "
    + SWLO.MinRng[i] + "-" + SWLO.MaxRng[i] + "\n-Ability - " + AbilityLookUpTable.ALUT.AbilityName[SWLO.AbilityNumber[i]] + ":" + AbilityLookUpTable.ALUT.AbilityBlurb[SWLO.AbilityNumber[i]] + "\n-Info: " + SWLO.ItemBlurb;


        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
