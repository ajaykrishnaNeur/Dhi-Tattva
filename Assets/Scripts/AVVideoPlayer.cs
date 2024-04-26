using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;

public class AVVideoPlayer : MonoBehaviour
{
    //public string videoFileName;
    public MediaPlayer mediaPlayer;
    public AVVideoDownloader videoDownloader;
    private APIManager apiManager;

    public string videoPath;
    private void Start()
    {
        apiManager = GameObject.Find("Api Manager").GetComponent<APIManager>();
    }
    public void PlayVideo()
    {
        videoPath = videoDownloader.savePath;

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }

        string fullPath = Path.Combine(Application.persistentDataPath, videoPath);

       bool isOpening = mediaPlayer.OpenMedia(new MediaPath(fullPath, MediaPathType.AbsolutePathOrURL), autoPlay: true);
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
    }
}


