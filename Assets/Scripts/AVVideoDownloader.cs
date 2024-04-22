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
    public string encryptionKey = "yourEncryptionKey";
    private const int KeySize = 256; // Key size in bits

    private byte[] DeriveKey(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(keyBytes);
            return hash.Take(KeySize / 8).ToArray(); // Take the first 256 bits (32 bytes) of the hash
        }
    }

    void Start()
    {
        int index = videoURL.IndexOf("videos/");

        if (index != -1)
        {
            videoUrlName = videoURL.Substring(index + "videos/".Length);
            Debug.Log(videoUrlName);
        }
        savePath = Path.Combine(Application.persistentDataPath, saveFileName);

        if (File.Exists(savePath))
        {
            Debug.Log("Video already exists locally at: " + savePath);
            PathText.text = savePath;
            aVVideoPlayer.PlayVideo();
        }
        else
        {
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
                if (progressSlider != null)
                {
                    progressSlider.value = www.downloadProgress;
                    slidervalue.text = progressSlider.value.ToString();
                }
                yield return null;
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string savePath = Path.Combine(Application.persistentDataPath, saveFileName);

                // Encrypt the downloaded video file
                EncryptVideo(savePath);

                Debug.Log("Video downloaded successfully to: " + savePath);
                PathText.text = savePath;
                aVVideoPlayer.PlayVideo();
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

    // Encrypts the downloaded video file
    private void EncryptVideo(string filePath)
    {
        const int bufferSize = 1024 * 1024; // 1 MB buffer size (adjust as needed)

        try
        {
            byte[] key = DeriveKey(encryptionKey); // Derive the key from a password

            string encryptedFilePath = filePath + ".enc";

            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (FileStream outputStream = new FileStream(encryptedFilePath, FileMode.Create, FileAccess.Write))
                {
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = new byte[aes.BlockSize / 8]; // Use all zeros IV for simplicity (not secure for production)

                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            while ((bytesRead = inputStream.Read(buffer, 0, bufferSize)) > 0)
                            {
                                cryptoStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }
            }

            Debug.Log("Video encrypted successfully: " + encryptedFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Encryption error: " + ex.Message);
        }
    }




    // Encrypts bytes using AES encryption with a given key
    private byte[] EncryptBytes(byte[] bytesToEncrypt, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = new byte[aes.BlockSize / 8]; // Use all zeros IV for simplicity (not secure for production)

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToEncrypt, 0, bytesToEncrypt.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }

}
