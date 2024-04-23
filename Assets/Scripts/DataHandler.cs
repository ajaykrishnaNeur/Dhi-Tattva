using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public GameObject welcomePanel;
    public GameObject verificationPanel;
    public GameObject AdminPanel;

    private APIManager apiManager;
    // Start is called before the first frame update
    void Start()
    {
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
        
    }

    public void JsonParserData(string _webData)
    {
       

    }
}
