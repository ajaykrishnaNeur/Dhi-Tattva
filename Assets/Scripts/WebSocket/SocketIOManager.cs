using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIOClient;
using System;
using TMPro;

public class SocketIOManager : MonoBehaviour
{
    private SocketIOUnity socket;
    public string serverUrl; // Replace with your server's address
    [Serializable]
    public class DeviceStatus
    {
        public string deviceId;
        public bool isOnline;
    }
    public class SynchDevice
    {
        public string deviceId;
        public bool isCompleted;
    }
   // public TextMeshProUGUI textMeshProUGUI;

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

        // Connect to the server
        socket.Connect();
        StartCoroutine(CheckConnectionTimeout());
      //  StartCoroutine(CallAfter());
       
        Debug.Log("deviceId is" + SystemInfo.deviceUniqueIdentifier);

    }

  
    IEnumerator CheckConnectionTimeout(float timeoutSeconds = 5f)
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
        ActiveDeviceStatus(true);
        

    }
    public void ActiveDeviceStatus(bool value)
    {

        DeviceStatus status = new DeviceStatus
        {
            deviceId = SystemInfo.deviceUniqueIdentifier,
            //deviceId ="abxcdfg",
            isOnline = value
        };

        socket.Emit("activeDevice", JsonUtility.ToJson(status)); //send as json
                                                                 //socket.Emit("activeDevice", status);// send as object

    }
    public void SynDevice()
    {
        SynchDevice synchDevice = new SynchDevice()
        {
            deviceId = SystemInfo.deviceUniqueIdentifier,
            isCompleted = true
        };

        socket.Emit("syncDevice", JsonUtility.ToJson(synchDevice));
        ActiveDeviceStatus(true);
    }
    public void SendDeviceID()
    {
        socket.Emit("activeDevice", SystemInfo.deviceUniqueIdentifier);
    }
    void OnDestroy()
    {
        // Properly disconnect the client when the object is destroyed
        socket.Disconnect();
    }
}
