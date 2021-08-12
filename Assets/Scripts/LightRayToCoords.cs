using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRayToCoords : MonoBehaviour
{
    public int UIDEPTH = -10;
    public static LightRayToCoords LRC;
    private Vector3 temp;
    private Camera cam;
    private UnityTemplateProjects.SimpleCameraController.CameraState camS;
    public Vector3 GetRayCamMouseXYZ()
    {

        if (Camera.main != cam)
        {
            cam = Camera.main;
            if (Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>() != null) //incase this is a static cam
            {
                camS = Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState;
            }
        }

        RaycastHit rh;
        RaycastHit rh2;

        // - new Vector3(Screen.width / 2, Screen.height / 2, 0)
        Vector3 screenPoint = new Vector3(cam.pixelWidth - Input.mousePosition.x, cam.pixelHeight - Input.mousePosition.y, DrawGrid.GridLines.DrawDepth * 2); //all tiles are -20? so make -40

        Vector3 screenPoint2 = new Vector3(cam.pixelWidth - Input.mousePosition.x, cam.pixelHeight - Input.mousePosition.y, -10); //UI1 is 5


        // temp = new Vector3(camS.x,camS.y,camS.z); //cam.ScreenToWorldPoint(new Vector3(mouseX, 0, mouseZ)) + 
        screenPoint = cam.ScreenToWorldPoint(screenPoint) + new Vector3(camS.x, camS.y, camS.z); //Camera.main.nearClipPlane
        screenPoint2 = cam.ScreenToWorldPoint(screenPoint2) + new Vector3(camS.x, camS.y, camS.z); //Camera.main.nearClipPlane
        screenPoint2.x /= 2;
        screenPoint2.z /= 2;

        screenPoint.x /= 2;
        screenPoint.z /= 2;
        //screenPoint.y = camS.y;




        if (Physics.Raycast(screenPoint, transform.TransformDirection(Camera.main.transform.forward), out rh, Mathf.Infinity))
        {
#if UNITY_EDITOR
            Debug.DrawRay(screenPoint, transform.TransformDirection(Camera.main.transform.forward) * rh.distance, Color.red);
#endif
            if (rh.collider.tag == "Selectable")
            {
#if UNITY_EDITOR
    
#endif
                if (Physics.Raycast(screenPoint2, transform.TransformDirection(Camera.main.transform.forward), out rh2, Mathf.Infinity) && rh2.collider.tag == "UI1")
                {
#if UNITY_EDITOR
            Debug.DrawRay(screenPoint2, transform.TransformDirection(Camera.main.transform.forward) * rh.distance, Color.red);
#endif
                    return new Vector3(-10000, -10000, -10000);
                }
                else
                {
                    return rh.point;
                }
            }
        }
        return new Vector3(-10000, -10000, -10000);
    }
    // Start is called before the first frame update
    void Start()
    {

        if (LRC == null)
        {
            LRC = this;
        }
        else if (LRC != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }
        DontDestroyOnLoad(LRC);

        cam = Camera.main;
        camS = Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState;
    }

    // Update is called once per frame
    void Update()
    {

//        if (Input.GetMouseButton(0))
//        {


            //GetRayCamMouseXYZ();

//        }
    }
}