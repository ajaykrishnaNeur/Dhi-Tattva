using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class DataHandler : MonoBehaviour
{
    public GameObject welcomePanel,Numpad,submitBtn,submitBtnVisual;
    public GameObject verificationPanel;
    public GameObject AdminPanel;
    public GameObject incorrectCodeText;
    public GameObject Ground;
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
        submitBtnVisual.SetActive(false);
        verificationPanel.SetActive(true);
    }

    public void WelcomePanelActive()
    {
        verificationPanel.SetActive(false);
        welcomePanel.SetActive(true);
    }
    public void WelcomePanelDisable()
    {
        welcomePanel.SetActive(false);
        Ground.SetActive(false);
    }
    public void WrongCredentialPanelActive()
    {
        incorrectCodeText.SetActive(true);
    }

    
    public void Login()
    {
        LoginCode loginCode = new LoginCode()
        {
            DeviceId = "8",
            //DeviceId = SystemInfo.deviceUniqueIdentifier,
            code = code_Inputfield.text,
        };

        string jsonData = JsonConvert.SerializeObject(loginCode);
        StartCoroutine(apiManager.LoginPostRequest("http://43.204.38.188:8000/v1/devices/register", jsonData));
    }
}
