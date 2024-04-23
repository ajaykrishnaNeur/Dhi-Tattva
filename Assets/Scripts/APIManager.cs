using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private DataHandler dataHandler;
    public string registerApi;
    private string Response;

    
    void Start()
    {
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
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
