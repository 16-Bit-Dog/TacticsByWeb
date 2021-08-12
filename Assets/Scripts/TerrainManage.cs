using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainManage : MonoBehaviour
{
    Vector3 adjustLineMin;
    Vector3 adjustLineMax;

    float DrawWidth = 0;
    float DrawHeight = 0;
    float DrawDepth = 0;

    public bool created = false;

    public GameObject LineObj;
    public GameObject TerrainT;

    
    public void Terrain()
    {
        if (created == false)
        {
            DrawWidth = 500; //:should be map size:
            DrawHeight = 500; //:should be map size: should be %20 == 0
            
            Camera.main.gameObject.GetComponent<UnityTemplateProjects.SimpleCameraController>().mouseCamSlide = true;
            Instantiate(TerrainT, new Vector3(0, 0, 0), Quaternion.identity);

            created = true;
        }
    }

    
    // Start is called before the first frame update
    void Start()
    {
        Terrain();
    }

    // Update is called once per frame
    void Update()
    {
        if (created == true)
        {
            DrawDepth = Input.mousePosition.y - 30;
           
            LineObj.transform.localScale += new Vector3(0, 0, DrawWidth * 10);

            for (float x = -1.0f * DrawWidth * 2.0f; x < DrawWidth * 2.0f; x += 20)
            {//TODO: ray cast down the vector 2 points to get line nicly as black on surface
                GameObject tmpOL = Instantiate(LineObj, new Vector3(x, 0, 0), Quaternion.identity);
                Destroy(tmpOL, 0.1f);
            }
            LineObj.transform.localScale += new Vector3(DrawWidth * 10, 0, -DrawWidth * 10);

            for (float y = -1.0f * DrawHeight * 2.0f; y < DrawHeight * 2.0f; y += 20)
            {
                GameObject tmpOL = Instantiate(LineObj, new Vector3(0, 0, y), Quaternion.identity);
                Destroy(tmpOL, 0.1f);
            }
            LineObj.transform.localScale += new Vector3(-DrawWidth * 10, 0, 0);
        }


    }
}
