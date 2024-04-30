using Newtonsoft.Json.Linq;
using Oculus.Platform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private DataHandler dataHandler;
    public string activeApi;
    private string deviceId;

    [SerializeField]
    public int videoCount;
    [SerializeField]
    public string adminId;
    public GameObject VideoDownload;
    public string[] GetVideoURL = new string[10], GetVideoName = new string[10];
    [SerializeField]
    public string id1, thumbnail1,description1,title1,urlvideo1;
    [SerializeField]
    public string id2, thumbnail2, description2, title2, urlvideo2;
    [SerializeField]
    public string packageId;
    public GameObject testcube;
    void Start()
    {
        //deviceId = "8";
        deviceId = SystemInfo.deviceUniqueIdentifier;
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        StartCoroutine(DeviceIdPostRequest(activeApi, deviceId));
    }

    public IEnumerator DeviceIdPostRequest(string url, string headername)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm());

        // Add custom header to the request
        request.SetRequestHeader("device-id", headername); 

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            if (request.downloadHandler.text.Contains("No active packages found."))
            {
                dataHandler.LoginPanelActive();
            }
            else
            {
                Debug.LogError("Error posting request: " + request.error);
                dataHandler.LoginPanelActive();
            }

        }

        else
        {
            Debug.Log("Request successful!");
            testcube.SetActive(false);
            string jsonResponse = request.downloadHandler.text;
            JObject jsonObject = JObject.Parse(jsonResponse);
            adminId = (string)jsonObject["adminId"];
            packageId = (string)jsonObject["activePackage"]["id"]; 
            // Get the 'videos' array from the JSON
            JArray videosArray = (JArray)jsonObject["activePackage"]["videos"];
            videoCount = videosArray.Count;
            Debug.Log("Number of videos: " + videoCount);
            for (int i = 0; i < videoCount; i++)
            {
                JObject video = (JObject)videosArray[i];

                if(i == 0)
                {
                     urlvideo1 = (string)video["url"];
                     GetVideoURL[0] = urlvideo1;
                     title1 = (string)video["title"];                    
                     description1 = (string)video["description"];
                     thumbnail1 = (string)video["thumbnail"];
                     id1 = (string)video["id"];
                     GetVideoName[0] = id1;
                }
                if (i == 1)
                {
                     urlvideo2 = (string)video["url"];
                     GetVideoURL[1] = urlvideo2;
                     title2 = (string)video["title"];
                     description2 = (string)video["description"];
                     thumbnail2 = (string)video["thumbnail"];
                     id2 = (string)video["id"];
                     GetVideoName[1] = id2;
                }
                
            }
            dataHandler.VerifiedPanelActive();
            VideoDownload.SetActive(true);
            Debug.Log("return-1" + jsonResponse);

        }
    }

    public IEnumerator LoginPostRequest(string url, string jsonData)
    {
        var req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
       
        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else // done
        {

            Debug.Log("return" + req.downloadHandler.text);
            string value = req.downloadHandler.text;
            
        }
        if (req.downloadHandler.text.Contains("Enter valid device register code"))
        {
            //INCORRECT CODE    
            dataHandler.WrongCredentialPanelActive();
        }

        if (req.downloadHandler.text.Contains("Device already paired"))
        {
            StartCoroutine(DeviceIdPostRequest(activeApi, deviceId));
        }
        if (req.downloadHandler.text.Contains(SystemInfo.deviceUniqueIdentifier))
        {
            StartCoroutine(DeviceIdPostRequest(activeApi, deviceId));
        }
    }

    public IEnumerator VideoCountPostRequest(string url, string jsonData)
    {
        var req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else // done
        {

            Debug.Log("return" + req.downloadHandler.text);
            string value = req.downloadHandler.text;

        }
       
    }

    public IEnumerator PackageCountPostRequest(string url, string jsonData)
    {
        var req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else // done
        {

            Debug.Log("return" + req.downloadHandler.text);
            string value = req.downloadHandler.text;

        }

    }
}
