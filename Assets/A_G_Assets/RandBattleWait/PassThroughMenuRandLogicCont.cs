using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassThroughMenuRandLogicCont : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (WebManage.WManage.StartMatchTToggle1 == true){
            LocalMatchInnerBase.LMIB.TakeRawWebCharDataAddToMap();
            LocalMatchInnerBase.LMIB.PassDataToIntermediateCS();
            SceneManager.LoadScene("BattleType1StartTrue"); 
        }
    }
}
