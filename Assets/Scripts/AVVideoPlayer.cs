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
    public void PlayVideo()
    {
        string videoPath = Path.Combine(Application.persistentDataPath, videoDownloader.videoName1);

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }

        string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoPath);

        bool isOpening = mediaPlayer.OpenMedia(new MediaPath(fullPath, MediaPathType.AbsolutePathOrURL), autoPlay: true);

        if (!isOpening)
        {
            Debug.LogError("Failed to open video: " + fullPath);
        }
    }
}


