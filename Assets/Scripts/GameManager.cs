using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject go;
    //private Ray r;
    private Vector3 temp;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0))
        {
            if (go != null)
            {
                go.transform.position = temp - new Vector3(0, 0, cam.transform.position.z * 2);
            }
        }else if (Input.GetMouseButtonUp(0) || Input.GetKey(KeyCode.A))
        {
            if (go != null)
            {
                go = null;
            }
        }
    }

    private void FixedUpdate()
    {
        RaycastHit rh;
        // - new Vector3(Screen.width / 2, Screen.height / 2, 0)
        float mouseX = cam.pixelWidth - Input.mousePosition.x;
        float mouseZ = cam.pixelHeight - Input.mousePosition.z;
        temp = cam.ScreenToWorldPoint(new Vector3(mouseX, mouseZ, mouseZ)) + cam.transform.position; 
        Debug.Log(temp);
        //Camera.main.transform.position - 
        if (Physics.Raycast(temp, transform.TransformDirection(Camera.main.transform.forward), out rh, Mathf.Infinity))
        {
            Debug.DrawRay(temp, transform.TransformDirection(Camera.main.transform.forward) * rh.distance, Color.red);
            if(rh.collider.tag == "Selectable")
            {
                go = rh.collider.gameObject;
            }
        }
        else
        {
            Debug.DrawRay(temp, transform.TransformDirection(Camera.main.transform.forward) * 10000, Color.yellow);

        }
    }
}
