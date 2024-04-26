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

    public string video1Name;
    public string video2Name;

    private List<string> videoNames = new List<string>();
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
        request.SetRequestHeader("device-id", headername); 

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

            string jsonResponse = request.downloadHandler.text;
            JObject jsonObject = JObject.Parse(jsonResponse);

            // Get the 'videos' array from the JSON
            JArray videosArray = (JArray)jsonObject["activePackage"]["videos"];

            // Count the number of videos
            videoCount = videosArray.Count;

            foreach (JObject video in videosArray)
            {
                string videoTitle = (string)video["title"];
                videoNames.Add(videoTitle);
            }

            // Output the video names
            for (int i = 0; i < videoCount; i++)
            {
                Debug.Log("Video Name:" +i + name);
                if(i == 0)
                {
                    video1Name = name;
                }
                if (i ==1)
                {
                    video2Name = name;
                }
            }
            // Output the video count
            Debug.Log("Number of videos: " + videoCount);

            dataHandler.VerifiedPanelActive();
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
            dataHandler.VerifiedPanelActive();
        }
        if (req.downloadHandler.text.Contains(SystemInfo.deviceUniqueIdentifier))
        {
            dataHandler.VerifiedPanelActive();

        }
    }
}
