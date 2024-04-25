using Oculus.Platform;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private DataHandler dataHandler;
    public string activeApi;
    private string Response;

    private string deviceId;

    void Start()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        StartCoroutine(DeviceIdPostRequest(activeApi, deviceId));
    }



    public IEnumerator DeviceIdPostRequest(string url, string headername)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm());

        // Add custom header to the request
        request.SetRequestHeader("device-id", headername); // Replace "Your-Header-Value" with the actual value

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error posting request: " + request.error);
            dataHandler.LoginPanelActive();
        }
        else
        {
            Debug.Log("Request successful!");
            dataHandler.welcomePanel.SetActive(true);
        }
    }

    public IEnumerator LoginPostRequest(string url, string jsonData)
    {
        var req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return req.SendWebRequest();
        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else // done
        {
            Debug.Log("return" + req.downloadHandler.text);
            string value = req.downloadHandler.text;
            dataHandler.VerifiedPanelActive();
        }
    }
}
