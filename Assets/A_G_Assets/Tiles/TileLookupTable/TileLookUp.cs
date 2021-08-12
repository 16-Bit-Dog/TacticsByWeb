using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TileLookUp.TLU.Tiles
public class TileLookUp : MonoBehaviour
{
    public static TileLookUp TLU;

    [System.Serializable]
    public class TilesC
    {
        public int ID = 0;
        public GameObject Obj;
        public string name = "";
        public int MoveCost = 0;
        public int Def = 0;
        public int Atk = 0;

        public int AtkBuffInitial = 0;
        public int AtkBuffEnd = 0;

        public int DefBuffInitial = 0;
        public int DefBuffEnd = 0;
        
        public int MovBuffEnd = 0;
        
        public int HPHeal = 0; //can be minus for posion - add anim for if net posion or net heal

        public int DmgInitial = 0;
        public int DmgEndTurn = 0;
    }


    public TilesC[] Tiles;

    
    //void CreateLookUpTable() //also has GameObject refrences
    //{
    //    for (int i = 0; i < Tiles.GetLength(0); i++)
    //    {
    //        Tiles[i] = new TilesC();
    //    }
    //
    //    Tiles[0].ID = 0;
    //    Tiles[0].Obj = TileForest1;
    //    Tiles[0].name = "Forest";
    //}
    

    // Start is called before the first frame update
    void Start()
    {
        if (TLU == null)
        {
            TLU = this;
        }
        else if (TLU != this)
        {
            //In case there is a different instance destroy this one.
            Destroy(this);
        }

        //Let the gameobject persist over the scenes logic - borrowed from stack overflow
        DontDestroyOnLoad(TLU);

     //   CreateLookUpTable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
