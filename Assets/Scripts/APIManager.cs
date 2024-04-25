using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private DataHandler dataHandler;
    public string activeApi;
    private string Response;

    
    void Start()
    {
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
       
    }

    //IEnumerator GetRequest(string url)
    //{
    //    using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
    //    {
    //        // Request and wait for the desired page.
    //        yield return webRequest.SendWebRequest();

    //        if (webRequest.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError("GET request failed: " + webRequest.error);
    //            dataHandler.LoginPanelActive();
    //        }
    //        else
    //        {
    //            // Print the received data
    //            Debug.Log("GET request successful: " + webRequest.downloadHandler.text);
    //        }
    //    }
    //}


    public IEnumerator DeviceIdPostRequest(string url, string jsonData)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("device-id", SystemInfo.deviceUniqueIdentifier);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("POST request failed: " + webRequest.error);
                dataHandler.LoginPanelActive();
            }
            else
            {
                Response = webRequest.downloadHandler.text;
            }

        }
    }

    public IEnumerator LoginPostRequest(string url, string jsonData)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("POST request failed: " + webRequest.error);
            }
            else
            {
                Response = webRequest.downloadHandler.text;
            }

        }
    }
}
