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
    public string videoURL;
    public string saveFileName; // Adjust the file name and extension as needed

    private string savePath;
    public TextMeshProUGUI PathText;
    public Slider progressSlider; // Reference to a UI slider for progress display

    public TextMeshProUGUI slidervalue;
    public AVVideoPlayer aVVideoPlayer;

    public GameObject sliderandText;


    //save video into url video name
    public string videoUrlName;
    public string encryptionKey = "yourEncryptionKey"; // Change this to your actual encryption key
    private const int KeySize = 256;

    public byte[] DeriveKey(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(keyBytes);
            return hash.Take(KeySize / 8).ToArray();
        }
    }

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(savePath))
        {
            Debug.Log("Video already exists locally at: " + savePath);
            PathText.text = savePath;
            aVVideoPlayer.PlayVideo(savePath);
        }
        else
        {
            StartCoroutine(DownloadVideoCoroutine());
        }
    }

    private IEnumerator DownloadVideoCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            www.downloadHandler = new DownloadHandlerFile(Path.Combine(Application.persistentDataPath, saveFileName));
            www.SendWebRequest();

            while (!www.isDone)
            {
                if (progressSlider != null)
                {
                    progressSlider.value = www.downloadProgress;
                }
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Video downloaded successfully to: " + savePath);

                string encryptedFilePath = EncryptVideo(savePath);
                Debug.Log("Video encrypted successfully: " + encryptedFilePath);

                PathText.text = encryptedFilePath;
                aVVideoPlayer.PlayVideo(encryptedFilePath);
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

    private string EncryptVideo(string filePath)
    {
        const int bufferSize = 1024 * 1024; // 1 MB buffer size

        try
        {
            byte[] key = DeriveKey(encryptionKey);
            string encryptedFilePath = filePath + ".enc";

            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
            using (Aes aes = Aes.Create())
            {
                // Generate a random initialization vector (IV)
                aes.IV = new byte[aes.BlockSize / 8];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(aes.IV);

                using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(key, aes.IV), CryptoStreamMode.Write))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = inputStream.Read(buffer, 0, bufferSize)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            return encryptedFilePath;
        }
        catch (Exception ex)
        {
            Debug.LogError("Encryption error: " + ex.Message);
            return string.Empty;
        }
    }
}

