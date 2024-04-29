using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public int count;
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
        VideoCount();
    }

    public void VideoCount()
    {
        LoginCode loginCode = new LoginCode()
        {

        };

        //string jsonData = JsonConvert.SerializeObject(loginCode);
        //apiManager.StartCoroutine(VideoCountPostRequest("http://43.204.38.188:8000/v1/devices/register", jsonData));
    }
}

