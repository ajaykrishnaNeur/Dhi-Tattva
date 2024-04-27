using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net.Sockets;

public class AVVideoDownloader : MonoBehaviour
{
    public TextMeshProUGUI[] pathTexts = new TextMeshProUGUI[10];
    public Slider progressSlider;
    public TextMeshProUGUI slidervalue;
    public AVVideoPlayer aVVideoPlayer;
    public APIManager apiManager;
    public DataHandler dataHandler;
    public GameObject socket;
    public string savePath1, savePath2;

    public bool isdownload;
    public int i;
    private void Start()
    {
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>(); 
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        for (i = 0; i < apiManager.videoCount; i++)
        {
            savePath1 = Path.Combine(Application.persistentDataPath, apiManager.GetVideoName[0]);
            savePath2 = Path.Combine(Application.persistentDataPath, apiManager.GetVideoName[1]);
            StartCoroutine(DownloadVideoCoroutine(apiManager.GetVideoURL[i], apiManager.GetVideoName[i]));
           
        }
    }
    

    IEnumerator DownloadVideoCoroutine(string videoURL, string videoName )
    {
        if(isdownload)
        {
            if (i == 0)
            {
                if (File.Exists(savePath1))
                {
                    Debug.Log("Video already exists locally at: " + savePath1);
                    //pathText.text = savePath;
                    dataHandler.WelcomePanelActive();
                    socket.SetActive(true);
                    //aVVideoPlayer.PlayVideo(); // Assuming you want to play the video if it already exists
                    yield break; // Exit the coroutine early
                }
            }
            if (i == 1)
            {
                if (File.Exists(savePath2))
                {
                    Debug.Log("Video already exists locally at: " + savePath2);
                    //pathText.text = savePath;
                    dataHandler.WelcomePanelActive();
                    socket.SetActive(true);
                    //aVVideoPlayer.PlayVideo(); // Assuming you want to play the video if it already exists
                    yield break; // Exit the coroutine early
                }
            }
           
        }
        // Check if the video file already exists locally


        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            if (i == 0)
            {
                www.downloadHandler = new DownloadHandlerFile(savePath1);
            }
            if (i == 1)
            {
                www.downloadHandler = new DownloadHandlerFile(savePath2);
            }
            www.SendWebRequest();

            while (!www.isDone)
            {
                // Update progress
                if (progressSlider != null)
                {
                    // Scale www.downloadProgress from 0-1 to 0-100
                    int scaledProgress = (int)(www.downloadProgress * 100);

                    // Assign the scaled progress value to progressSlider
                    progressSlider.value = scaledProgress;

                    // Update slidervalue text
                    slidervalue.text = scaledProgress.ToString();
                }
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                progressSlider.value = 100;
                Debug.Log("Video downloaded successfully to: " + savePath1);
                isdownload = true;
                dataHandler.WelcomePanelActive();
                socket.SetActive(true);
                //pathText.text = savePath;
                //aVVideoPlayer.PlayVideo(); // Play the video after downloading
         }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

 
}
