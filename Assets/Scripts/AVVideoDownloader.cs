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
    private bool video1Downloaded = false;
    private bool video2Downloaded = false;
    public int video2;
    private void Start()
    {
        video2 = 0;
        Debug.Log(SystemInfo.deviceUniqueIdentifier);
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        for (int i = 0; i < apiManager.videoCount; i++)
        {
            // Construct save paths for videos
            string videoName = apiManager.GetVideoName[i];

            // Skip if video name is null or empty
            if (string.IsNullOrEmpty(videoName))
            {
                Debug.LogError("Invalid video name for index " + i);
                continue;
            }
            savePath1 = Path.Combine(Application.persistentDataPath, apiManager.GetVideoName[0]);
            savePath2 = Path.Combine(Application.persistentDataPath, apiManager.GetVideoName[1]);
            //string savePath = Path.Combine(Application.persistentDataPath, videoName);

            // Check if the video file already exists locally
            if (PlayerPrefs.GetInt(videoName, 0) == 1 && File.Exists(savePath1) && File.Exists(savePath2))
            {

                    Debug.Log("Video is fully downloaded: " + videoName); 
                    UpdateVideoStatus(i);
            }
            else
            {
                // If the video doesn't exist locally, start downloading
                StartCoroutine(DownloadVideoCoroutine(apiManager.GetVideoURL[i], videoName, i));
            }
        }

        // Check if both videos are downloaded
        if (video1Downloaded && video2Downloaded)
        {
            // If both videos are downloaded, activate the welcome panel and socket
            dataHandler.WelcomePanelActive();
            socket.SetActive(true);
        }
    }

    IEnumerator DownloadVideoCoroutine(string videoURL, string videoName, int videoIndex)
    {
        string savePath = Path.Combine(Application.persistentDataPath, videoName);

        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            www.downloadHandler = new DownloadHandlerFile(savePath);
            www.SendWebRequest();

            while (!www.isDone)
            {
                    if (progressSlider != null)
                    {
                        // Scale www.downloadProgress from 0-1 to 0-100
                        int scaledProgress = (int)(www.downloadProgress * 100 /2 + video2);

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
                Debug.Log("Video downloaded successfully to: " + savePath);
                PlayerPrefs.SetInt(videoName, 1);
                PlayerPrefs.Save();
                UpdateVideoStatus(videoIndex);
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

    // Update the download status of the video and check if both videos are downloaded
    private void UpdateVideoStatus(int videoIndex)
    {
        if (videoIndex == 0)
        {
            video1Downloaded = true;
        }
        else if (videoIndex == 1)
        {
            video2Downloaded = true;
            //video2 = 50;
        }

        // Check if both videos are downloaded
        if (video1Downloaded && video2Downloaded)
        {

            // If both videos are downloaded, activate the welcome panel and socket
            dataHandler.WelcomePanelActive();
            socket.SetActive(true);
        }
    }

}
