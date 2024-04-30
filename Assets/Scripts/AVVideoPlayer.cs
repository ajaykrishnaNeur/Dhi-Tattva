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

    public string videoPath;
    public GameObject sphere;

    private double videoDuration;
    private bool isVideoEnded;
    private bool isvideo1Count, isVideo2Count;
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

    private void Update()
    {
        if (socketIOManager != null && socketIOManager.isVideo1 == true)
        {
            videoDuration = mediaPlayer.Info.GetDuration() / 60;
            double currentTime = mediaPlayer.Control.GetCurrentTime() / 60;
            if (videoDuration != 0 && currentTime >= videoDuration)
            {
                isvideo1Count = true;
                VideoCountAdd videoCountAdd = new VideoCountAdd()
                {
                    //videoId = "662b925da7cc1803b45f08a2",
                    //adminId = "662f6104cb488c1bf137a4c6",
                    videoId = apiManager.id1,
                    adminId = apiManager.adminId,
                    count = "1"
                    
                };
              
                string jsonData = JsonConvert.SerializeObject(videoCountAdd);
                apiManager.StartCoroutine(apiManager.VideoCountPostRequest("http://43.204.38.188:8000/v1/video-counts", jsonData));
            }

        }
       else  if (socketIOManager != null && socketIOManager.isVideo2 == true )
        {
            videoDuration = mediaPlayer.Info.GetDuration() / 60;
            double currentTime = mediaPlayer.Control.GetCurrentTime() / 60;
            if (videoDuration != 0 && currentTime >= videoDuration)
            {
                isVideo2Count = true;
                VideoCountAdd videoCountAdd = new VideoCountAdd()
                {
                    //videoId = "662b925da7cc1803b45f08a2",
                    //adminId = "662f6104cb488c1bf137a4c6",
                    videoId = apiManager.id2,
                    adminId = apiManager.adminId,
                    count = "1"

                };

                string jsonData = JsonConvert.SerializeObject(videoCountAdd);
                apiManager.StartCoroutine(apiManager.VideoCountPostRequest("http://43.204.38.188:8000/v1/video-counts", jsonData));
            }

        }


        if (isvideo1Count && isVideo2Count)
        {
            PackageCount();
            isvideo1Count = false;
            isVideo2Count = false;
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

    public void VideoCount()
    {
        VideoCountAdd videoCountAdd = new VideoCountAdd()
        {
            //videoId = "662b925da7cc1803b45f08a2",
            //adminId = "662f6104cb488c1bf137a4c6",
            videoId = apiManager.id1,
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
            packageId = apiManager.packageId,
            adminId = apiManager.adminId,
            count = "1"

        };

        string jsonData = JsonConvert.SerializeObject(packageCountAdd);
        apiManager.StartCoroutine(apiManager.PackageCountPostRequest("http://43.204.38.188:8000/v1/package-counts", jsonData));
    }

}

