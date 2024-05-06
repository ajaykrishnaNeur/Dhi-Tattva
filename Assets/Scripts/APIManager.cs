using Newtonsoft.Json.Linq;
using Oculus.Platform;
using SimpleJSON;
using SocketIOClient.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{

    public string activeApi;
    public GameObject VideoDownload;
    private string deviceId;

    public  DataHandler dataHandler;
    [HideInInspector]
    public int videoCount;
    [HideInInspector]
    public string adminId;
    [HideInInspector]
    public string[] GetVideoURL = new string[10], GetVideoName = new string[10];
    [HideInInspector]
    public string id1, thumbnail1,description1,title1,urlvideo1;
    [HideInInspector]
    public string id2, thumbnail2, description2, title2, urlvideo2;
    [HideInInspector]
    public string packageId;
    private string message;
    private void Awake()
    {
        deviceId = SystemInfo.deviceUniqueIdentifier;
    }
    void Start()
    {
        //deviceId = "8";
       
     //   dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        StartCoroutine(DeviceIdPostRequest(activeApi, deviceId));
    }

    public IEnumerator DeviceIdPostRequest(string url, string headername)
    {
        yield return new WaitForSeconds(2f);
        var req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(headername);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("device-id", headername);
        //  req.certificateHandler = new BypassCertificateHandler();
        //Send the request then wait here until it returns
        yield return req.SendWebRequest();
        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);

            if (req.downloadHandler.text.Contains("No active packages found."))
            {
                dataHandler.LoginPanelActive();
                dataHandler.apiMessage.text = "No active packages found.";
            }
            else
            {
                Debug.LogError("Error posting request: " + req.error);
                dataHandler.LoginPanelActive();
            }
        }
        else // done
        {
            Debug.Log("Request successful!");
            string jsonResponse = req.downloadHandler.text;
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

                if (i == 0)
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

            JObject json = JObject.Parse(value);
            message = (string)json["message"];

        }
       

        if (req.downloadHandler.text.Contains("Device already paired") || req.downloadHandler.text.Contains(SystemInfo.deviceUniqueIdentifier))
        {
            StartCoroutine(DeviceIdPostRequest(activeApi, deviceId));
            dataHandler.WrongCredentialPanelActive();
            dataHandler.apiMessage.text = "";
        }
        else 
        {
            //INCORRECT CODE    
            dataHandler.WrongCredentialPanelActive();
            dataHandler.apiMessage.text = message;          
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
