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
    public MediaPlayer mediaPlayer;
    public AVVideoDownloader videoDownloader;
    public void PlayVideo()
    {
        string videoPath = Path.Combine(Application.persistentDataPath, videoDownloader.videoUrlName);

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }

        bool isEncrypted = IsVideoEncrypted(videoPath);

        if (isEncrypted)
        {
            DecryptVideo(videoPath);
        }

        bool isOpening = mediaPlayer.OpenMedia(new MediaPath(videoPath, MediaPathType.AbsolutePathOrURL), autoPlay: true);

        if (!isOpening)
        {
            Debug.LogError("Failed to open video: " + videoPath);
        }
    }

    private bool IsVideoEncrypted(string filePath)
    {
        return Path.GetExtension(filePath).Equals(".enc", System.StringComparison.OrdinalIgnoreCase);
    }

    private void DecryptVideo(string filePath)
    {
        try
        {
            byte[] encryptedBytes = File.ReadAllBytes(filePath);
            byte[] decryptedBytes = DecryptBytes(encryptedBytes, videoDownloader.encryptionKey);
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


