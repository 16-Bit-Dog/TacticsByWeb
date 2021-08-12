using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriWLookup : MonoBehaviour //PriWLookup.PWLO
{
    public static PriWLookup PWLO;
    
    public List<GameObject> PriWeapons; //I can fill with empty game objects rather than models for place holding - I may not make 2 sets of models
    public List<string> Name; //must be the same size as the SecWeapons
    public List<Sprite> Image;

    public List<int> Atk; //must be the same size as the priWeapons
    public List<int> Def; //must be the same size as the priWeapons
    public List<int> Hp; //must be the same size as the priWeapons
    public List<int> Mov; //must be the same size as the priWeapons
    public List<int> MinRng; //must be the same size as the priWeapons
    public List<int> MaxRng; //must be the same size as the priWeapons
    public List<int> MainAnimNum; //must be the same size as the priWeapons

    public List<string> ItemBlurb;

    public List<string> StatsAndInfo; //must be the same size as the priWeapons
    // Start is called before the first frame update

    public List<int> AbilityNumber;
    // number

    void Awake() { }

    void Start()
    {
        if (PWLO == null)
        {
            PWLO = this;
        }
        else if (PWLO != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(PWLO);

        for (int i = 0; i < PWLO.StatsAndInfo.Count; i++)
        {

            PWLO.StatsAndInfo[i] = "-Name: " + PWLO.Name[i] + "\n-Move: "+ PWLO.Mov[i]+"\n-HP: "+ PWLO.Hp[i] + "\n-Atk: " + PWLO.Atk[i] + "\n-Def: " + PWLO.Def[i] + "\n-Range: "
                + PWLO.MinRng[i] + "-" + PWLO.MaxRng[i] + "\n-Ability - " + AbilityLookUpTable.ALUT.AbilityName[PWLO.AbilityNumber[i]]+ ": " + AbilityLookUpTable.ALUT.AbilityBlurb[PWLO.AbilityNumber[i]] + "\n-Info: " + PWLO.ItemBlurb;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
