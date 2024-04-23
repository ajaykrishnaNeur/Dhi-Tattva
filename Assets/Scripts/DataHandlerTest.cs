using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static APIManagerTest;
using SimpleJSON;

public class DataHandlerTest : MonoBehaviour
{
    public GameObject LoginPage;
    public GameObject CorrectCredentialPage;
    public GameObject WrongCredentialPage;

    public TMP_InputField username;
    public TMP_InputField password;

    public APIManagerTest api;

    public TextMeshProUGUI messageText, Statustext;

    public void LoginBtn()
    {
        Login login = new Login();
        login.login = username.text;
        login.password = password.text;

        string jsonDataLogin = JsonUtility.ToJson(login);
        StartCoroutine(api.PostRequest(api.logingApi, jsonDataLogin));
    }

    private void CorrectCredential()
    {
        CorrectCredentialPage.SetActive(true);
        LoginPage.SetActive(false);
    }

    private void WrongCredential()
    {
        WrongCredentialPage.SetActive(true);
        LoginPage.SetActive(false);
    }

    public void JsonParserData(string _webData)
    {
        // Parse the JSON data
        var jsonObject = JSONNode.Parse(_webData);

        string status = jsonObject["status"];
        string message = jsonObject["message"];

        if (status == "error")
        {
            WrongCredential();
            messageText.text = "message - "+message;
            Statustext.text = "status - " + status;
        }
        else
        {
            CorrectCredential();
            messageText.text = "message - "+message;
            Statustext.text = "status - " + status;
        }

    }
}
