using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantCharObject : MonoBehaviour //ConstantCharObject.CCObj.CharObjConstant
{
    //ConstantCharObject.CCObj.headString

    public static ConstantCharObject CCObj;


    public string headString = "Armature/Bone/Bone.001/Bone.023/Bone.036/Bone.002/Bone.003/Bone.004";

    public string WHandString = "Armature/Bone/Bone.001/Bone.023/Bone.036/Bone.002/Bone.007/Bone.008/Bone.009/Bone.010/Bone.011/Bone.012/Bone.013/Bone.014";

    public GameObject CharObjConstant;

    public Material blue;
    public Material red;
    public Material green;
    public Material yellow;
    public Material purple;

    public Material Selected;

    public Material UsedUpMat;

    public List<Material> matArr = new List<Material>();


    public Material blueHard;
    public Material redHard;
    public Material greenHard;
    public Material yellowHard;
    public Material purpleHard;

    public List<Material> matArrHard = new List<Material>();

    public GameObject Bar;
    public Material BackBar;


    // Start is called before the first frame update
    void Start()
    {


        if (CCObj == null)
        {
            CCObj = this;
        }
        else if (CCObj != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

        DontDestroyOnLoad(CCObj);
        //Let the gameobject persist over the scenes logic - borrowed from stack overflow


        CCObj.matArr.Add(CCObj.blue);
        CCObj.matArr.Add(CCObj.red);
        CCObj.matArr.Add(CCObj.green);
        CCObj.matArr.Add(CCObj.yellow);
        CCObj.matArr.Add(CCObj.purple);

        CCObj.matArrHard.Add(CCObj.blueHard);
        CCObj.matArrHard.Add(CCObj.redHard);
        CCObj.matArrHard.Add(CCObj.greenHard);
        CCObj.matArrHard.Add(CCObj.yellowHard);
        CCObj.matArrHard.Add(CCObj.purpleHard);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
