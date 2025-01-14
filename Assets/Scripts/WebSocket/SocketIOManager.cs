using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using TMPro;
using SimpleJSON;
using UnityEngine.Video;

public class SocketIOManager : MonoBehaviour
{
    private SocketIOUnity socket;
    public string serverUrl; 

    public AVVideoPlayer avVideoPlayer;
    public AVVideoDownloader videoDownloader;
    public APIManager apiManager;
    public bool isPlay,isRestart,isPause;
    public bool isVideo1, isVideo2;
    public bool isFirst1,isFirst2;
    public GameObject sphere;
    public string packageId,videoId;
    void Start()
    {
        BoolInitialize();
        // Setup the Socket.IO connection
        var uri = new Uri(serverUrl);
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                {"token", "UNITY"}
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += (sender, e) => {
            Debug.Log("Successfully connected to the server.");
        };

        socket.OnDisconnected += (sender, e) => {
            Debug.LogError("Disconnected from server. Reason: " + e);
        };
        // Listen for events from the server
        socket.OnUnityThread("connection", (response) =>
        {

            Debug.Log("Connected to the server");
            Debug.Log(response.GetValue<string>());

        });

        socket.OnUnityThread("spin", (response) =>
        {
            // objectToSpin.transform.Rotate(0, 45, 0);
            Debug.Log("dinesh");
        });
        socket.OnError += (sender, e) =>
        {

            Debug.LogError("Connection failed: " + e);
        };

        // Connect to the server
        socket.Connect();
        StartCoroutine(CheckConnectionTimeout());
        ReciveDeviceDetails();

    }

    public void BoolInitialize()
    {
        isFirst1 = true;
        isFirst2 = true;
        isPlay = false;
        isPause = false;
        isRestart = false;
        isVideo2 = false;
        isVideo1 = false;
    }
  
    IEnumerator CheckConnectionTimeout(float timeoutSeconds = 60f)
    {
        yield return new WaitForSeconds(timeoutSeconds);

        // Check if connected, if not, log a timeout error
        if (!socket.Connected)
        {
            Debug.LogError("Connection attempt timed out.");
        }
    }
 
    public void ReciveDeviceDetails()
    {

        socket.On("commandTracker", (response) =>
        {
            // Extract the JSON string from the array
            string jsonString = response.GetValue<string>(0);

            // Parse the JSON string
            var jsonObject = JSON.Parse(jsonString);

            // Access individual properties
             bool play = jsonObject["play"].AsBool;
             bool restart = jsonObject["restart"].AsBool;
             videoId = jsonObject["video_id"].Value;
             packageId = jsonObject["package_id"].Value;

            if (play && !restart)
            {
                Debug.Log("played");
                isPlay = true;
               
            }
            if (!play && !restart)
            {
                Debug.Log("paused");
                isPause = true;
            }
            if (!play && restart)
            {               
                Debug.Log("restarted");
                isRestart = true;
            }

        });

    }
    void OnDestroy()
    {
        socket.Disconnect();
    }

    private void Update()
    {
        if(apiManager.packageId == packageId)
        {
            if (isPlay)
            {
                if(videoId == apiManager.id1)
                {
                    if (isFirst1)
                    {
                        avVideoPlayer.videoPath = videoDownloader.savePath1;
                        avVideoPlayer.StartPlay();
                        avVideoPlayer.PlayVideo();
                        isFirst1 = false;
                        isFirst2 = true;
                    }                    
                    isVideo1 = true;
                    isVideo2 = false;
                    avVideoPlayer.ResumeVideo();
                    isPlay = false;
                }
                else if(videoId == apiManager.id2)
                {
                    if (isFirst2)
                    {
                        avVideoPlayer.videoPath = videoDownloader.savePath2;
                        avVideoPlayer.StartPlay();
                        avVideoPlayer.PlayVideo();
                        isFirst1 = true;
                        isFirst2 = false;
                    }                  
                    isVideo1 = false;
                    isVideo2 = true;
                    avVideoPlayer.ResumeVideo();
                    isPlay = false;
                }
               
            }
            if (isPause)
            {
                if (videoId == apiManager.id1 && isVideo1)
                {
                    avVideoPlayer.PauseVideo();                   
                }
                else if (videoId == apiManager.id2 && isVideo2)
                {
                    avVideoPlayer.PauseVideo();
                }
                isPause = false;
            }

            if (isRestart)
            {
                if (videoId == apiManager.id1 && isVideo1)
                {
                    avVideoPlayer.RestartVideo();
                }
                else if (videoId == apiManager.id2 && isVideo2)
                {
                    avVideoPlayer.RestartVideo();                    
                }
                isRestart = false;
            }
        }
  
        
    }

}
