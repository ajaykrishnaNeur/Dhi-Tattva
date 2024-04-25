using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

public class AVVideoDownloader : MonoBehaviour
{
    public string videoURL = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    public string saveFileName = "data.bin"; // Rename the file to something less conspicuous
    public TextMeshProUGUI PathText;

    public void Start()
    {
        StartCoroutine(DownloadAndSaveVideoCoroutine());
    }

    IEnumerator DownloadAndSaveVideoCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                byte[] videoData = www.downloadHandler.data;

                // Save the video data with an obfuscated file name
                string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
                File.WriteAllBytes(savePath, videoData);

                Debug.Log("Video downloaded and saved successfully to: " + savePath);
                PathText.text = savePath;
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }
}

