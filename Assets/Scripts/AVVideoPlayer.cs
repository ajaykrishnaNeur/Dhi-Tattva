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
    public SocketIOManager socketIOManager;
    [HideInInspector]
    public string videoPath;
    public GameObject sphere;

    private double videoDuration;
    private bool isVideoEnded;
    private bool isvideo1Count, isVideo2Count;
    private bool isVideo1CountExit, isVideo2CountExit;
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
        isVideo1CountExit = true;
        isVideo2CountExit = true;
        dataHandler = GameObject.Find("Data Handler").GetComponent<DataHandler>();
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
    }

    private void Update()
    {
        if(apiManager.videoCount == 2)
        {
            Video2Package();
        }
        if (apiManager.videoCount == 1)
        {
            Video1Package();
        }
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

        if (socketIOManager.isVideo1 == true)
        {
            double currentTime = mediaPlayer.Control.GetCurrentTime() /60;
            double videoDuration = mediaPlayer.Info.GetDuration()/60;
            double endTime = videoDuration;
            Debug.LogError("duration1:" + endTime);
        }
        else if(socketIOManager.isVideo2 == true)
        {
            double videoDuration = mediaPlayer.Info.GetDuration();
            double endTime = videoDuration;
            Debug.LogError("duration2:" + endTime);
        }



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

    public void PackageCount()
    {
        PackageCountAdd packageCountAdd = new PackageCountAdd()
        {
            packageId = apiManager.packageId,
            adminId = apiManager.adminId,
            count = "1"

        };

        string jsonData = JsonConvert.SerializeObject(packageCountAdd);
        apiManager.StartCoroutine(apiManager.PackageCountPostRequest("http://43.204.38.188:8000/v1/package-counts", jsonData));
    }

    public void Video2Package()
    {
        if (socketIOManager != null && socketIOManager.isVideo1 == true)
        {

            videoDuration = mediaPlayer.Info.GetDuration() / 60;
            double currentTime = mediaPlayer.Control.GetCurrentTime() / 60;
            if (currentTime == 0)
            {
                isVideo1CountExit = true;
            }
            if (videoDuration != 0 && currentTime >= videoDuration && isVideo1CountExit)
            {
                isvideo1Count = true;
                VideoCountAdd videoCountAdd = new VideoCountAdd()
                {
                    videoId = apiManager.id1,
                    adminId = apiManager.adminId,
                    count = "1"

                };

                string jsonData = JsonConvert.SerializeObject(videoCountAdd);
                apiManager.StartCoroutine(apiManager.VideoCountPostRequest("http://43.204.38.188:8000/v1/video-counts", jsonData));
                isVideo1CountExit = false;
            }

        }
        else if (socketIOManager != null && socketIOManager.isVideo2 == true)
        {
            videoDuration = mediaPlayer.Info.GetDuration() / 60;
            double currentTime = mediaPlayer.Control.GetCurrentTime() / 60;
            if (currentTime == 0)
            {
                isVideo2CountExit = true;
            }
            if (videoDuration != 0 && currentTime >= videoDuration && isVideo2CountExit)
            {
                isVideo2Count = true;
                VideoCountAdd videoCountAdd = new VideoCountAdd()
                {
                    videoId = apiManager.id2,
                    adminId = apiManager.adminId,
                    count = "1"

                };

                string jsonData = JsonConvert.SerializeObject(videoCountAdd);
                apiManager.StartCoroutine(apiManager.VideoCountPostRequest("http://43.204.38.188:8000/v1/video-counts", jsonData));
                isVideo2CountExit = false;
            }

        }


        if (isvideo1Count && isVideo2Count)
        {
            PackageCount();
            isvideo1Count = false;
            isVideo2Count = false;
        }
    }

    public void Video1Package()
    {
        if (socketIOManager != null && socketIOManager.isVideo1 == true)
        {

            videoDuration = mediaPlayer.Info.GetDuration() ;
            double currentTime = mediaPlayer.Control.GetCurrentTime();
            //Debug.Log("currenttime:" + currentTime);
            if (currentTime == 0|| currentTime == 1|| currentTime ==2)
            {
                isVideo1CountExit = true;
            }
            if (videoDuration != 0 && currentTime >= videoDuration-0.6)
            {
                isvideo1Count = true;
                VideoCountAdd videoCountAdd = new VideoCountAdd()
                {
                    videoId = apiManager.id1,
                    adminId = apiManager.adminId,
                    count = "1"

                };
                mediaPlayer.Control.Seek(0f);
                mediaPlayer.Play();
                string jsonData = JsonConvert.SerializeObject(videoCountAdd);
                apiManager.StartCoroutine(apiManager.VideoCountPostRequest("http://43.204.38.188:8000/v1/video-counts", jsonData));
                isVideo1CountExit = false;
            }

        }

        if (isvideo1Count)
        {
            PackageCount();
            isvideo1Count = false;
        }
    }
}

