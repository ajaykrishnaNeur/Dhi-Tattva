using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Video;

public class AVVideoDownloader : MonoBehaviour
{
    public string videoURL;
    //public string saveFileName; // Adjust the file name and extension as needed

    private string savePath1,savePath2;
    public TextMeshProUGUI pathText1,pathText2;
    public Slider progressSlider; // Reference to a UI slider for progress display

    public TextMeshProUGUI slidervalue;
    public AVVideoPlayer aVVideoPlayer;
    public string videoName1,videoName2;

    public APIManager apiManager;
    public int videoCount;
    void Start()
    {
        
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
        videoName1 = apiManager.title1;
        videoCount = apiManager.videoCount;
        videoURL = apiManager.urlvideo1;
        if (videoCount == 2)
        {
            savePath1 = Path.Combine(Application.persistentDataPath, videoName1);
            // Check if the video file already exists locally
            if (File.Exists(savePath1))
            {
                Debug.Log("Video already exists locally at: " + savePath1);
                pathText1.text = savePath1;
                aVVideoPlayer.GetComponent<AVVideoPlayer>().PlayVideo();
            }
            else
            {
                // If the video doesn't exist locally, download it
                StartCoroutine(DownloadVideo1Coroutine());
            }
        }
        if (videoCount == 0)
        {
            savePath1 = Path.Combine(Application.persistentDataPath, videoName1);
            savePath2 = Path.Combine(Application.persistentDataPath, videoName2);
            // Check if the video file already exists locally
            if (File.Exists(savePath1) && File.Exists(savePath2))
            {
                Debug.Log("Video1 already exists locally at: " + savePath1);
                Debug.Log("Video2 already exists locally at: " + savePath2);
                pathText1.text = savePath1;
                pathText2.text = savePath2;
                aVVideoPlayer.GetComponent<AVVideoPlayer>().PlayVideo();
            }
            else
            {
                // If the video doesn't exist locally, download it
                StartCoroutine(DownloadVideo1Coroutine());
            }
        }

    }

    IEnumerator DownloadVideo1Coroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            www.downloadHandler = new DownloadHandlerFile(Path.Combine(Application.persistentDataPath, videoName1));
            www.SendWebRequest();

            while (!www.isDone)
            {
                // Update progress
                if (progressSlider != null)
                {
                    // Scale www.downloadProgress from 0-1 to 0-100
                    int scaledProgress = (int)(www.downloadProgress * 100)/2;

                    // Assign the scaled progress value to progressSlider
                    progressSlider.value = scaledProgress;

                    // Update slidervalue text
                    slidervalue.text = (scaledProgress/2).ToString();
                }
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string savePath = Path.Combine(Application.persistentDataPath, videoName1);
                Debug.Log("Video downloaded successfully to: " + savePath);
                pathText1.text = savePath;
                //aVVideoPlayer.PlayVideo();
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

}
