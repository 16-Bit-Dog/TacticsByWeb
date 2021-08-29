using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

public class Login_Set : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_InputField username;

    public TMP_Text Error;

    [Serializable]
    class dataLoginInfo{
        public string ps = "0";// = WebManage.WManage.LoginPassword; // password data

        public string us = "0";// = WebManage.WManage.LoginUsername; // username data

        public string es = "0";
    }

    string LoginInfoSaverString(){
        dataLoginInfo data = new dataLoginInfo();

        data.ps = WebManage.WManage.LoginPassword; // password data

        data.us = WebManage.WManage.LoginUsername; // username data

        data.es = WebManage.WManage.email;

        return JsonConvert.SerializeObject(data);
    }
    // Start is called before the first frame update
    void Start()
    {

        FileInfo fileInfo = new FileInfo(Application.persistentDataPath
                     + "/AutoLogin/");

        if (!fileInfo.Directory.Exists){ 
            fileInfo.Directory.Create(); //make not existing file path
            
            WebManage.WManage.email = "a";
            WebManage.WManage.LoginPassword = "a";
            WebManage.WManage.LoginUsername = "a";
            File.WriteAllText(Application.persistentDataPath
                + "/MapsLocalSave/" + "AutoLogin" + ".dat", LoginInfoSaverString());
        
        }
        else{
            try{
            Debug.Log("PreLoad");

            dataLoginInfo data = JsonConvert.DeserializeObject<dataLoginInfo>(File.ReadAllText(Application.persistentDataPath
                + "/MapsLocalSave/" + "AutoLogin" + ".dat"));

            WebManage.WManage.email = data.es;
            WebManage.WManage.LoginPassword = data.ps;
            WebManage.WManage.LoginUsername = data.us;
            
            password.text = data.ps;
            username.text = data.us;
            email.text = data.es;

            }

            catch{
                WebManage.WManage.email = "a";
                WebManage.WManage.LoginPassword = "a";
                WebManage.WManage.LoginUsername = "a";
                File.WriteAllText(Application.persistentDataPath
                    + "/MapsLocalSave/" + "AutoLogin" + ".dat", LoginInfoSaverString());
        
            }
        }
    
    }

    // Update is called once per frame
    void Update()
    {

        WebManage.WManage.email = email.text; //send email data

        WebManage.WManage.LoginPassword = password.text; //send password data

        WebManage.WManage.LoginUsername = username.text; //send username data

    }

    IEnumerator LoginDelay()
    {

        while (WebManage.WManage.GetLoginID)
        {
            yield return null;
        }
        if (WebManage.WManage.ErrorAquired)
        {
            Error.text = WebManage.WManage.ErrorMessage;
            WebManage.WManage.GetLoginID = false;
            WebManage.WManage.ErrorAquired = false;
            //show email text error
        }
        else
        {
            StaticDataMapMaker.controlObj.LoadMain();
            WebManage.WManage.GetLoginID = false;

        }
    }

    public void Login()
    {
        File.WriteAllText(Application.persistentDataPath
            + "/MapsLocalSave/" + "AutoLogin" + ".dat", LoginInfoSaverString());
        
        if (WebManage.WManage.GetLoginID == false)
        {
            WebManage.WManage.GetLoginID = true;

            IEnumerator LoginDelayr = LoginDelay();

            StartCoroutine(LoginDelayr);
        }
    }

    IEnumerator CreateAccountDelay()
    {

        while (WebManage.WManage.CreateNewAccountBool == false)
        {
            yield return null;
        }
        if (WebManage.WManage.ErrorAquired)
        {
            Error.text = WebManage.WManage.ErrorMessage;
            WebManage.WManage.CreateNewAccountBool = false;
            WebManage.WManage.ErrorAquired = false;
            //show email text error
        }
        else
        {
            StaticDataMapMaker.controlObj.LoadMain();
            WebManage.WManage.CreateNewAccountBool = false;
            
        }
    }

    public void CreateAccount()
    {
        if (WebManage.WManage.CreateNewAccountBool == false)
        {
            WebManage.WManage.CreateNewAccountBool = true;

            IEnumerator CreateAccountDelayr = CreateAccountDelay();
        }
    }

}
