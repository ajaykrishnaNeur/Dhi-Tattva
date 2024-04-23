using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.Events;

public class APIManagerTest : MonoBehaviour
{

    string apiUrl = "https://jsonplaceholder.typicode.com/posts";
    public string logingApi= "https://recruitment-api.pyt1.stg.jmr.pl/login";
    private string Response;
    public string message, statuss;

    public DataHandler dataHandler;
    public class Login
    {
        public string login;
        public string password;
    }
   
    void Start()
    {
        //StartCoroutine(GetRequest(apiUrl)); 
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("GET request failed: " + webRequest.error);
            }
            else
            {
                // Print the received data
                Debug.Log("GET request successful: " + webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator PostRequest(string url, string jsonData)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("POST request failed: " + webRequest.error);
            }
            else
            {
                Response = webRequest.downloadHandler.text;
                dataHandler.JsonParserData(Response);
            }
            
        }
    }
       
}


