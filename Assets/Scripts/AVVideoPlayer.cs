using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using static DataHandler;

public class AVVideoPlayer : MonoBehaviour
{
    public MediaPlayer mediaPlayer;
    private APIManager apiManager;
    private DataHandler dataHandler;

    public string videoPath;
    public GameObject sphere;
    public class VideoCountAdd
    {
        public string videoId;
        public string adminId;
        public string count;
    }

    public class PackageCountAdd
    {
        public string packageId;
        public string adminId;
        public string count;
    }
    private void Start()
    {
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
    }

    public void PlayVideo()
    {
       

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }
        string fullPath = Path.Combine(Application.persistentDataPath, videoPath);
        bool isOpening = mediaPlayer.OpenMedia(new MediaPath(fullPath, MediaPathType.AbsolutePathOrURL));

        double videoDuration = mediaPlayer.Info.GetDuration();
        double endTime = videoDuration;
        Debug.LogError("duration:" + endTime);

        if (!isOpening)
        {
            Debug.LogError("Failed to open video: " + fullPath);
        }

        
    }
    
    public void PauseVideo()
    {
        mediaPlayer.Play();
        mediaPlayer.Pause();
    }
    public void ResumeVideo()
    {
        mediaPlayer.Play();
    }
    public void RestartVideo()
    {
        mediaPlayer.Control.Seek(0f);
        mediaPlayer.Play();
    }

    public void StartPlay()
    {
        sphere.SetActive(true);
        RestartVideo();
        ResumeVideo();
        dataHandler.WelcomePanelDisable();
    }

    public void VideoCount()
    {
        VideoCountAdd videoCountAdd = new VideoCountAdd()
        {
            videoId = "662b925da7cc1803b45f08a2",
            adminId = "662f6104cb488c1bf137a4c6",
            count = "1"

        };

        string jsonData = JsonConvert.SerializeObject(videoCountAdd);
        apiManager.StartCoroutine(apiManager.VideoCountPostRequest("http://43.204.38.188:8000/v1/video-counts", jsonData));
    }

    public void PackageCount()
    {
        PackageCountAdd packageCountAdd = new PackageCountAdd()
        {
            packageId = "662b92f3a7cc1803b45f08bc",
            adminId = "662f6104cb488c1bf137a4c6",
            count = "1"

        };

        string jsonData = JsonConvert.SerializeObject(packageCountAdd);
        apiManager.StartCoroutine(apiManager.PackageCountPostRequest("http://43.204.38.188:8000/v1/package-counts", jsonData));
    }

}

