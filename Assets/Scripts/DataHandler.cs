using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{
    public GameObject welcomePanel,Numpad,submitBtn;
    public GameObject verificationPanel;
    public GameObject AdminPanel;
    public GameObject incorrectCodeText;
    public TMP_InputField code_Inputfield;
    private APIManager apiManager;

    public class LoginCode
    {
        public string code;
        public string DeviceId;
    }
    // Start is called before the first frame update
    void Start()
    {
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
    }

    public void LoginPanelActive()
    {
        AdminPanel.SetActive(true);
        Numpad.SetActive(true);
        submitBtn.SetActive(true);
    }

    public void VerifiedPanelActive()
    {
        AdminPanel.SetActive(false);
        Numpad.SetActive(false);
        submitBtn.SetActive(false);
        verificationPanel.SetActive(true);
    }

    public void WelcomePanelActive()
    {
        verificationPanel.SetActive(false);
        welcomePanel.SetActive(true);
    }
    public void WrongCredentialPanelActive()
    {
        incorrectCodeText.SetActive(true);
    }

    
    public void Login()
    {
        LoginCode loginCode = new LoginCode()
        {
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            code = code_Inputfield.text,
        };

        string jsonData = JsonConvert.SerializeObject(loginCode);
        StartCoroutine(apiManager.LoginPostRequest("http://43.204.38.188:8000/v1/devices/register", jsonData));
    }
}
