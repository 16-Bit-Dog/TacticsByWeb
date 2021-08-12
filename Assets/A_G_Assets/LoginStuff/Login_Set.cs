using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;

public class Login_Set : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_InputField username;

    public TMP_Text Error;

    // Start is called before the first frame update
    void Start()
    {

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
