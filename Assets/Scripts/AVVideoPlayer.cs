using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using System.Security.Cryptography;
using System.Text;
using System;

public class AVVideoPlayer : MonoBehaviour
{
    //public string videoFileName;
    public MediaPlayer mediaPlayer;
    public AVVideoDownloader videoDownloader;
    public void PlayVideo()
    {
        string videoPath = Path.Combine(Application.persistentDataPath, videoDownloader.saveFileName);

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }

        bool isEncrypted = IsVideoEncrypted(videoPath);

        if (isEncrypted)
        {
            DecryptVideo(videoPath, videoDownloader.encryptionKey);
        }

        bool isOpening = mediaPlayer.OpenMedia(new MediaPath(videoPath, MediaPathType.AbsolutePathOrURL), autoPlay: true);

        if (!isOpening)
        {
            Debug.LogError("Failed to open video: " + videoPath);
        }
    }

    private bool IsVideoEncrypted(string filePath)
    {
        return File.Exists(filePath + ".enc");
    }

    private void DecryptVideo(string filePath, string key)
    {
        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(filePath + ".enc");
            byte[] decryptedBytes = DecryptBytes(encryptedBytes, key);
            File.WriteAllBytes(filePath, decryptedBytes);
            Debug.Log("Video decrypted successfully: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Decryption error: " + ex.Message);
        }
    }

    private byte[] DecryptBytes(byte[] bytesToDecrypt, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[aes.BlockSize / 8];

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToDecrypt, 0, bytesToDecrypt.Length);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }

}


