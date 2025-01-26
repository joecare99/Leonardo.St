using System;
using System.IO;
using System.Security.Cryptography;

namespace Leonardo.Models;

public class EncryptionUtility
{
    private static readonly byte[] Key;

    private static readonly byte[] IV;

    static EncryptionUtility()
    {
        Key = Convert.FromBase64String("J5V+chcqR4BiKdEcMsC0wVjaW94ESa8/NLa28D3+ChI=");
        IV = Convert.FromBase64String("GyN51+n33DLArsVvF0GhNw==");
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        byte[] array = new byte[length];
        using RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
        rNGCryptoServiceProvider.GetBytes(array);
        return array;
    }

    public static byte[] EncryptStringToBytes_Aes(string plainText)
    {
        if (plainText == null || plainText.Length <= 0)
        {
            throw new ArgumentNullException("plainText");
        }
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;
        ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new MemoryStream();
        using CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
        using (StreamWriter streamWriter = new StreamWriter(stream))
        {
            streamWriter.Write(plainText);
        }
        return memoryStream.ToArray();
    }

    public static string DecryptStringFromBytes_Aes(byte[] cipherText)
    {
        if (cipherText == null || cipherText.Length == 0)
        {
            throw new ArgumentNullException("cipherText");
        }
        string text = null;
        using Aes aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;
        ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream stream = new MemoryStream(cipherText);
        using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
        using StreamReader streamReader = new StreamReader(stream2);
        return streamReader.ReadToEnd();
    }
}
