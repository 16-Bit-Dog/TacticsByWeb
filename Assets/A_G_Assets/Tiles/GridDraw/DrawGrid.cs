using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    
    public static DrawGrid GridLines;

    public GameObject LineObj;

    public float DrawDepth = -20;

    public float DrawWidth = 0;
    public float DrawHeight = 0;

    public void UpdateDrawGridWidthBasedMap()
    {
        GridLines.DrawWidth = StaticDataMapMaker.controlObj.MapWidth;
    }
    public void UpdateDrawGridWidthSolidVal(int i)
    {
        GridLines.DrawWidth = i;
    }

    public void UpdateDrawGridHeightBasedMap()
    {
        GridLines.DrawHeight = StaticDataMapMaker.controlObj.MapHeight;
    }
    public void UpdateDrawGridHeightSolidVal(int i)
    {
        GridLines.DrawHeight = i;
    }
    public List<GameObject> LineObjA;

    public void DeleteGrid()
    {
        for (int i = 0; i < GridLines.LineObjA.Count; i++)
        {
            Destroy(GridLines.LineObjA[i]);
        }

        GridLines.LineObjA.Clear();
    }

    public void UpdateGridDraw()
    {
        DeleteGrid();
        //TODO:
        GridLines.LineObj.transform.localScale += new Vector3(0, 0, GridLines.DrawWidth * 20);

        for (float x = 0; x < GridLines.DrawWidth + 1; x += 1)
        {//TODO: ray cast down the vector 2 points to get line nicly as black on surface
            GridLines.LineObjA.Add( Instantiate(GridLines.LineObj, new Vector3(x * 4 - 2, GridLines.DrawDepth, 0), Quaternion.identity) );
            //   Destroy(tmpOL, 0.1f);
        }
        GridLines.LineObj.transform.localScale += new Vector3(GridLines.DrawWidth * 20, 0, -GridLines.DrawWidth * 20);

        for (float y = 0; y < GridLines.DrawHeight + 1; y += 1)
        {
            GridLines.LineObjA.Add( Instantiate(GridLines.LineObj, new Vector3(0, GridLines.DrawDepth, y * 4 - 2), Quaternion.identity) );
            //     Destroy(tmpOL, 0.1f);
        }
        GridLines.LineObj.transform.localScale += new Vector3(-GridLines.DrawWidth * 20, 0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        GridLines = this;

        if (GridLines == null)
        {
            GridLines = this;
        }
        else if (GridLines != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

        //Let the gameobject persist over the scenes logic - borrowed from stack overflow
        DontDestroyOnLoad(GridLines);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

