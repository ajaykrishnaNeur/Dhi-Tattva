using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using RenderHeads.Media.AVProVideo;
using System.Security.Cryptography;
using System;

public class VideoPlayerNOEnc : MonoBehaviour
{
    public string obfuscatedFileName = "data.bin"; // Use the same obfuscated file name as in AVVideoDownloaderObfuscated
    public MediaPlayer mediaPlayer;
    private string encryptionKey; // Same key used for encryption
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
    }
    public void PlayObfuscatedVideo()
    {
        string obfuscatedFilePath = Path.Combine(Application.persistentDataPath, obfuscatedFileName);

        if (!File.Exists(obfuscatedFilePath))
        {
            Debug.LogError("Obfuscated video file not found: " + obfuscatedFilePath);
            return;
        }

        if (!mediaPlayer)
        {
            Debug.LogError("No MediaPlayer assigned!");
            return;
        }

        // Decrypt the obfuscated video file
        byte[] decryptedData = Decrypt(File.ReadAllBytes(obfuscatedFilePath), encryptionKey);

        // Play the decrypted video
        string fullPath = "file://" + Path.Combine(Application.persistentDataPath, "decryptedVideo.mp4"); // Use a temporary file to store decrypted video
        File.WriteAllBytes(fullPath, decryptedData); // Save decrypted video to a temporary file
        bool isOpening = mediaPlayer.OpenMedia(new MediaPath(fullPath, MediaPathType.AbsolutePathOrURL), autoPlay: true);

        if (!isOpening)
        {
            Debug.LogError("Failed to open video: " + fullPath);
        }
    }

    // AES decryption method
    private byte[] Decrypt(byte[] data, string key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(key);
            aesAlg.Mode = CipherMode.CBC;

            // Extract IV from the beginning of the data
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            Array.Copy(data, 0, iv, 0, iv.Length);
            aesAlg.IV = iv;

            // Create decryptor
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            // Create memory stream to write decrypted data
            using (MemoryStream msDecrypt = new MemoryStream())
            {
                // Create crypto stream to perform decryption
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    // Write decrypted data to the stream
                    csDecrypt.Write(data, iv.Length, data.Length - iv.Length);
                    csDecrypt.FlushFinalBlock();
                }

                return msDecrypt.ToArray();
            }
        }
    }
}


