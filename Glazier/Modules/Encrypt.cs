using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Glazier.Modules;
public static class Encrypt
{
    public static async Task<string> EncryptAsync(string cleartext, string password)
    {
        var hasher = SHA256.Create();
        var key = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Padding = PaddingMode.PKCS7;
        var iv = aes.IV;

        var byteStream = new MemoryStream(10000);
        byteStream.Write(iv, 0, iv.Length);

        using var cryptoStream = new CryptoStream(
            byteStream,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write);

        var encryptWriter = new StreamWriter(cryptoStream);
        await encryptWriter.WriteAsync(cleartext);
        encryptWriter.Close();

        var bytes = byteStream.ToArray();
        var base64 = Convert.ToBase64String(bytes);

        return base64;
    }
    public static async Task<string> DecryptAsync(string ciphertext, string password)
    {
        var hasher = SHA256.Create();
        var key = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));

        var encryptedArray = Convert.FromBase64String(ciphertext);
        var byteStream = new MemoryStream(encryptedArray);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.Padding = PaddingMode.PKCS7;
        var iv = new byte[aes.IV.Length];
        var numBytesToRead = aes.IV.Length;
        var numBytesRead = 0;
        while (numBytesToRead > 0)
        {
            var n = byteStream.Read(iv, numBytesRead, numBytesToRead);
            if (n == 0) break;

            numBytesRead += n;
            numBytesToRead -= n;
        }

        using var cryptoStream = new CryptoStream(
               byteStream,
               aes.CreateDecryptor(key, iv),
               CryptoStreamMode.Read);

        var decryptReader = new StreamReader(cryptoStream);
        var decryptedMessage = await decryptReader.ReadToEndAsync();

        return decryptedMessage;
    }
}
