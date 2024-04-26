using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using TMPro;
using SimpleJSON;

public class SocketIOManager : MonoBehaviour
{
    private SocketIOUnity socket;
    public string serverUrl; // Replace with your server's address

    public AVVideoPlayer player;
    void Start()
    {
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

        // Set the serializer (if needed, for example if you're using IL2CPP and need to use Newtonsoft)
        // socket.JsonSerializer = new NewtonsoftJsonSerializer();
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
        //  StartCoroutine(CallAfter());
        ReciveDeviceDetails();
        //Debug.Log("deviceId is" + SystemInfo.deviceUniqueIdentifier);

    }

  
    IEnumerator CheckConnectionTimeout(float timeoutSeconds = 15f)
    {
        yield return new WaitForSeconds(timeoutSeconds);

        // Check if connected, if not, log a timeout error
        if (!socket.Connected)
        {
            Debug.LogError("Connection attempt timed out.");
        }
    }
    // Example of sending a message to the server when the space key is pressed
    void Update()
    {
       
        

    }
  /*  public void ActiveDeviceStatus(bool value)
    {

        DeviceStatus status = new DeviceStatus
        {
            deviceId = SystemInfo.deviceUniqueIdentifier,
            //deviceId ="abxcdfg",
            isOnline = value
        };

        socket.Emit("commandTracker", JsonUtility.ToJson(status)); //send as json
                                                                 //socket.Emit("activeDevice", status);// send as object

    }*/
 
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
            string videoId = jsonObject["video_id"].Value;
            string packageId = jsonObject["package_id"].Value;

            // Now you can use these values as needed
            //Debug.Log("Received commandTracker event - Play: " + play + ", Restart: " + restart + ", Video ID: " + videoId + ", Package ID: " + packageId);
            if(play && !restart)
            {
                Debug.Log("played");

            }
            if (!play && restart)
            {
                Debug.Log("restarted");
            }
            if (!play && !restart)
            {
                Debug.Log("paused");
            }
        });

    }
    void OnDestroy()
    {
        // Properly disconnect the client when the object is destroyed
        socket.Disconnect();
    }
}
