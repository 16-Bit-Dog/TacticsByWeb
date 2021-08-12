using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;

public class CharDataEditor
{
    public string name = ""; //name... also ending for file name

    public int pW = 0; //primary weapon
    public int sW = 0; //secondary weapon

    public int cH = 0;//hat cosmetic
    public int cB = 0; //body cosmetic

}

public class CharMakerMakerObj : MonoBehaviour
{
    Animator animPlayer;

    public CharDataEditor Cdata = new CharDataEditor();
    public static CharMakerMakerObj co;

    public GameObject charObjP;

    public GameObject SecondaryWeapon;
    public GameObject PrimaryWeapon;
    public GameObject SWText;
    public GameObject PWText;

    public GameObject CosViewBody;
    public GameObject CosViewHat;
    public GameObject CVBText;
    public GameObject CVHText;

    public GameObject NameInput;
    TMP_InputField inputFieldN;

    public GameObject InfoTextGUI;
    public TMP_Text InfoText; //should be null

    public string StatsAndInfo; //should be null

    GameObject charObj;

    public GameObject hatObj; //should be null
    Transform head;

    public GameObject bodyObj; //should be null

    Vector3 RefPos = new Vector3(0, 0, 0);
    Vector3 NewPos = new Vector3(0, 0, 0);
    Vector3 rot = new Vector3(0, 0, 0);
    float slowDown = 0.01f;

    public void ChangeCharName()
    {

        co.Cdata.name = inputFieldN.text;

    }

    public void SaveCharacter()
    {
        FileInfo fileInfo = new FileInfo(Application.persistentDataPath
                        + "/Chars/");

        if (!fileInfo.Directory.Exists) fileInfo.Directory.Create(); //make not existing file path

        StaticDataMapMaker.SaveCharData data = new StaticDataMapMaker.SaveCharData();

        Debug.Log(co.Cdata.pW);
        Debug.Log(co.Cdata.sW);
        Debug.Log(co.Cdata.cH);
        Debug.Log(co.Cdata.cB);
        Debug.Log(co.Cdata.name);

        data.pW = co.Cdata.pW;
        data.sW = co.Cdata.sW;
        data.cH = co.Cdata.cH;
        data.cB = co.Cdata.cB;
        data.name = co.Cdata.name;


        File.WriteAllText(Application.persistentDataPath
                        + "/Chars/" + co.Cdata.name + ".dat", JsonConvert.SerializeObject(data));


        SceneManager.LoadScene("Main");

    }

    public void ToggleOffOnCosOrWeap()
    {
        if (SecondaryWeapon.active)
        {
            SecondaryWeapon.SetActive(false);
            PrimaryWeapon.SetActive(false);
            SWText.SetActive(false);
            PWText.SetActive(false);

            CosViewBody.SetActive(true);
            CosViewHat.SetActive(true);
            CVBText.SetActive(true);
            CVHText.SetActive(true);
        }
        else
        {
            SecondaryWeapon.SetActive(true);
            PrimaryWeapon.SetActive(true);
            SWText.SetActive(true);
            PWText.SetActive(true);

            CosViewBody.SetActive(false);
            CosViewHat.SetActive(false);
            CVBText.SetActive(false);
            CVHText.SetActive(false);
        }



    }

    // Start is called before the first frame update
    void Start()
    {
        co = this;

        inputFieldN = NameInput.GetComponent<TMP_InputField>();

        charObj = Instantiate(ConstantCharObject.CCObj.CharObjConstant, new Vector3(-4, 0, 0), Quaternion.identity);

        if (StaticDataMapMaker.controlObj.CharPassCH != null || StaticDataMapMaker.controlObj.CharPassCH != 0)
        {
            hatObj = Instantiate(HatLookup.HLO.Hat[StaticDataMapMaker.controlObj.CharPassCH], new Vector3(0, 0, 0), Quaternion.identity); //new Vector3(-4, 4.5f, 0)
        }

        inputFieldN.text = StaticDataMapMaker.controlObj.CharPassName;
        co.Cdata.pW = StaticDataMapMaker.controlObj.CharPassPW;
        co.Cdata.sW = StaticDataMapMaker.controlObj.CharPassSW;
        co.Cdata.cH = StaticDataMapMaker.controlObj.CharPassCH;
        co.Cdata.cB = StaticDataMapMaker.controlObj.CharPassCB;
        co.Cdata.name = StaticDataMapMaker.controlObj.CharPassName;

        InfoText = InfoTextGUI.GetComponent<TMP_Text>();


        animPlayer = charObj.GetComponent<Animator>();


        head = co.charObj.transform.Find(ConstantCharObject.CCObj.headString);

        animPlayer.SetTrigger("IdleMaker");

        if(head == null)
        {

            Debug.Log("BAD");

        }
    
    }

    // Update is called once per frame
    void Update()
    {
        if (co.hatObj != null)
        {
            co.hatObj.transform.SetParent(head, false);
            //co.hatObj.transform.position = ;

    //        co.hatObj.transform.eulerAngles = head.eulerAngles;
        }
        //animPlayer.SetTrigger("");

        ChangeCharName();

        if (Input.GetMouseButton(0))
        {
            NewPos = (Input.mousePosition - RefPos);

            rot.y = -(NewPos.x + NewPos.y) * slowDown;

            charObj.transform.Rotate(rot);//Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

            //RefPos = NewPos;
        }
        else
        {

            RefPos = Input.mousePosition;

        }

    }
}
