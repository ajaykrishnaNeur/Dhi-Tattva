using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;
using static DataHandler;

public class DataHandler : MonoBehaviour
{
    public GameObject welcomePanel,Numpad,submitBtn;
    public GameObject verificationPanel;
    public GameObject AdminPanel;

    public TMP_InputField code_Inputfield;
    private APIManager apiManager;

    public class DeviceIdFetch
    {
        public string DeviceId;
    }

    public class LoginCode
    {
        public string code;
        public string DeviceId;
    }
    // Start is called before the first frame update
    void Start()
    {
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
        DeviceId();
    }

    public void DeviceId()
    {
        DeviceIdFetch deviceIdFetch = new DeviceIdFetch();
        deviceIdFetch.DeviceId = SystemInfo.deviceUniqueIdentifier;

        string jsonData = JsonConvert.SerializeObject(deviceIdFetch);
        StartCoroutine(apiManager.DeviceIdPostRequest("http://43.204.38.188:8000/v1/packages/active_package", jsonData));
    }

    public void JsonParserData(string _webData)
    {


    }

    public void LoginPanelActive()
    {
        AdminPanel.SetActive(true);
        Numpad.SetActive(true);
        submitBtn.SetActive(true);
    }

    public void Login()
    {
        LoginCode loginCode = new LoginCode();
        loginCode.DeviceId = SystemInfo.deviceUniqueIdentifier;
        loginCode.code = code_Inputfield.text;

        string jsonData = JsonConvert.SerializeObject(loginCode);
        StartCoroutine(apiManager.LoginPostRequest("http://43.204.38.188:8000/v1/packages/active_package", jsonData));
    }
}
