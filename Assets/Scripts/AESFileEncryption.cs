using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public static class AESFileEncryption
{
    private static readonly byte[] Key = new byte[32] // 256-bit key
    {
        0x4e, 0x69, 0x6e, 0x65, 0x54, 0x65, 0x65, 0x6e,
        0x41, 0x45, 0x53, 0x4b, 0x65, 0x79, 0x41, 0x6e,
        0x64, 0x49, 0x76, 0x65, 0x63, 0x74, 0x6f, 0x72,
        0x23, 0x46, 0x6f, 0x72, 0x41, 0x45, 0x53, 0x45
    };

    private static readonly byte[] IV = new byte[16] // 128-bit IV
    {
         0x43, 0x6f, 0x64, 0x65, 0x20, 0x54, 0x6f, 0x20,
         0x41, 0x45, 0x53, 0x20, 0x49, 0x56, 0x00, 0x45
    };

    public static byte[] EncryptData(byte[] data)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(data, 0, data.Length);
                    csEncrypt.FlushFinalBlock();

                    return msEncrypt.ToArray();
                }
            }
        }
    }

    public static void DecryptFile(string inputFile, string outputFile)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open))
            {
                using (FileStream fsDecrypted = new FileStream(outputFile, FileMode.Create))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(fsInput, decryptor, CryptoStreamMode.Read))
                    {
                        csDecrypt.CopyTo(fsDecrypted);
                    }
                }
            }
        }
    }
}
