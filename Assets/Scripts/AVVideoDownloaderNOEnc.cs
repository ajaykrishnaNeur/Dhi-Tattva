using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System;

public class AVVideoDownloaderNOEnc : MonoBehaviour
{
    public string videoURL = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    public string obfuscatedFileName = "data.bin"; // Obfuscated file name
    public TextMeshProUGUI PathText;
    public Slider progressSlider; // Progress slider reference
   private string encryptionKey;

    private string GenerateEncryptionKey()
    {
        // Generate a random 128-bit key (16 bytes)
        byte[] keyBytes = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(keyBytes);
        }
        return Convert.ToBase64String(keyBytes);
    }
    public void Start()
    {
        encryptionKey = GenerateEncryptionKey();
        StartCoroutine(DownloadAndSaveVideoCoroutine());
    }

    IEnumerator DownloadAndSaveVideoCoroutine()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(videoURL))
        {
            // Get video size for calculating progress
            www.SendWebRequest();
            while (!www.isDone)
            {
                float progress = Mathf.Clamp01(www.downloadProgress / 0.9f); // Divide by 0.9f to get a value between 0 and 1
                progressSlider.value = progress;
                yield return null;
            }

            yield return www;

            if (www.result == UnityWebRequest.Result.Success)
            {
                byte[] videoData = www.downloadHandler.data;

                // Encrypt the video data before saving
                byte[] encryptedData = Encrypt(videoData, encryptionKey);

                // Save the encrypted video data with the obfuscated file name
                string savePath = Path.Combine(Application.persistentDataPath, obfuscatedFileName);
                File.WriteAllBytes(savePath, encryptedData);

                Debug.Log("Video downloaded, encrypted, and saved successfully to: " + savePath);
                PathText.text = savePath;
            }
            else
            {
                Debug.Log("Error downloading video: " + www.error);
            }
        }
    }

    private byte[] Encrypt(byte[] data, string key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(key);
            aesAlg.Mode = CipherMode.CBC;

            // Generate IV
            aesAlg.GenerateIV();
            byte[] iv = aesAlg.IV;

            // Create encryptor
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create memory stream to write encrypted data
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Write IV to the beginning of the stream
                msEncrypt.Write(iv, 0, iv.Length);

                // Create crypto stream to perform encryption
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    // Write encrypted data to the stream
                    csEncrypt.Write(data, 0, data.Length);
                    csEncrypt.FlushFinalBlock();
                }

                return msEncrypt.ToArray();
            }
        }
    }

}
