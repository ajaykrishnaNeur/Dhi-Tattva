using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AVVideoDownloader : MonoBehaviour
{
    public string videoURL;
    public string saveFileName; // Adjust the file name and extension as needed

    private string savePath;
    public TextMeshProUGUI PathText;
    public Slider progressSlider; // Reference to a UI slider for progress display

    public TextMeshProUGUI slidervalue;
    public AVVideoPlayer aVVideoPlayer;

    public GameObject sliderandText;
    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        // Check if the video file already exists locally
        if (File.Exists(savePath))
        {
            Debug.Log("Video already exists locally at: " + savePath);
            PathText.text = savePath;
            //sliderandText.SetActive(false);
            aVVideoPlayer.GetComponent<AVVideoPlayer>().PlayVideo();
        }
        else
        {
            // If the video doesn't exist locally, download it
            StartCoroutine(DownloadVideoCoroutine());
        }
    }

    IEnumerator DownloadVideoCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            www.downloadHandler = new DownloadHandlerFile(Path.Combine(Application.persistentDataPath, saveFileName));
            www.SendWebRequest();

            while (!www.isDone)
            {
                // Update progress
                if (progressSlider != null)
                {
                    progressSlider.value = www.downloadProgress;
                    slidervalue.text= progressSlider.value.ToString();
                }
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string savePath = Path.Combine(Application.persistentDataPath, saveFileName);
                Debug.Log("Video downloaded successfully to: " + savePath);
                PathText.text = savePath;
                //sliderandText.SetActive(false);
                aVVideoPlayer.GetComponent<AVVideoPlayer>().PlayVideo();
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

}
