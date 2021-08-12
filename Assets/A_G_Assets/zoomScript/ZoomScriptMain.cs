using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomScriptMain : MonoBehaviour
{
    public static ZoomScriptMain ZSM;

    public float ZoomFactor = 0;

    public float SCROLLFACTOR = 1;

    private UnityTemplateProjects.SimpleCameraController.CameraState CamS;

    // Start is called before the first frame update
    void Start()
    {
        ZSM = this;

        CamS = Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState;

    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            ZSM.ZoomFactor += Input.GetAxis("Mouse ScrollWheel") / ZSM.SCROLLFACTOR;

            if (ZSM.ZoomFactor > 10)
            {

                ZSM.ZoomFactor = 10;

            }
            else if(ZSM.ZoomFactor < -50)
            {

                ZSM.ZoomFactor = -50;

            }
            else
            {

                CamS.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") / ZSM.SCROLLFACTOR));

            }

        }
        DrawGrid.GridLines.DrawDepth = -20 + ZoomScriptMain.ZSM.ZoomFactor;
    
    
    }


}
