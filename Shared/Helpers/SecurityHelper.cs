using System.Security.Cryptography;
using System.Text;

namespace API.Shared.Helpers;

public static class SecurityHelper
{
    private const string EncryptionKey = "YourEncryptionKey32BytesLongXXXX";

    public static string GenerateSalt()
    {
        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    public static string GetHashedPassword(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static string Encrypt(string plainText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = new byte[16];
            
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = new byte[16];
            
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }
}

