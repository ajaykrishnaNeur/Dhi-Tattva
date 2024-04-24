using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Linq;

public class AVVideoPlayer : MonoBehaviour
{
    //public string videoFileName;
    public MediaPlayer mediaPlayer;
    public AVVideoDownloader videoDownloader;
    private string encryptionKey = "yourEncryptionKey"; // Change this to your actual encryption key

    public void PlayVideo(string encryptedFilePath)
    {
        string decryptedPath = DecryptVideo(encryptedFilePath);

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }

        if (decryptedPath != null)
        {
            mediaPlayer.OpenMedia(new MediaPath(encryptedFilePath, MediaPathType.AbsolutePathOrURL), autoPlay: true);
        }
        else
        {
            Debug.LogError("Failed to decrypt video.");
        }
    }

    private string DecryptVideo(string filePath)
    {
        try
        {
            byte[] key = DeriveKey(encryptionKey);
            string decryptedFilePath = Path.Combine(Application.persistentDataPath, "decrypted_video.mp4");

            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.Write))
            using (Aes aes = Aes.Create())
            {
                // Generate a random initialization vector (IV)
                aes.IV = new byte[aes.BlockSize / 8];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(aes.IV);

                using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(key, aes.IV), CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(outputStream);
                }
            }

            return decryptedFilePath;
        }
        catch (Exception ex)
        {
            Debug.LogError("Decryption error: " + ex.Message);
            return null;
        }
    }

    private byte[] DeriveKey(string password)
    {
        const int KeySize = 256;
        using (var sha256 = SHA256.Create())
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(keyBytes);
            return hash.Take(KeySize / 8).ToArray();
        }
    }
}


