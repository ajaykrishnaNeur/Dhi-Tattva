using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TutorialAPI : MonoBehaviour
{
    public class UserDetails
    {
        public int userId;
        public int id;
        public string title;
        public bool completed;
    }
    public class Login
    {
        public string DeviceId;
        public string code;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WebGetRequest("https://jsonplaceholder.typicode.com/todos/1"));
        Login loginData = new Login()
        {
            DeviceId = SystemInfo.deviceUniqueIdentifier,
            code = "654363",
          // optional, defaults to 60
        };
        string jsonData = JsonUtility.ToJson(loginData);

        StartCoroutine(PostRequest("http://43.204.38.188:8000/v1/devices/register", jsonData));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator WebGetRequest(string api)
    {
        var apireq = api;
        string jsonData;
        var req = new UnityWebRequest(apireq, "GET"); //Create a new UnityWebRequest object for making a GET request
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
      

        yield return req.Send();
        if (req.isNetworkError) // error in request
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else 
        {
           
            jsonData = req.downloadHandler.text;
            UserDetails userDetails=JsonUtility.FromJson<UserDetails>(jsonData);
      

            Debug.Log("UserDetails : " + userDetails.title); 
            Debug.Log("UserID : " + userDetails.userId);
            Debug.Log("Status : " + userDetails.completed);
            Debug.Log("Id : " + userDetails.id);
        }


    }
    IEnumerator PostRequest(string api, string jsonData)
    {

        var req = new UnityWebRequest(api, "POST");   
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData); 
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend); 
                                                                             
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
       //  req.certificateHandler = new BypassCertificateHandler();
        //Send the request then wait here until it returns
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
