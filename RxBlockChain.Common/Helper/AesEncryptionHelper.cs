using System.Security.Cryptography;
using System.Text;

namespace RxBlockChain.Common.Helper
{
    public static class AesEncryptionHelper
    {
        //To be improved soon, will use IOptions to retrieve env variables
        private static readonly string _encryptionKey = Environment.GetEnvironmentVariable("AES_ENCRYPTION_KEY");

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(_encryptionKey))
                throw new InvalidOperationException("Encryption key not set.");

            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] iv = new byte[16];

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var writer = new StreamWriter(cryptoStream);
                writer.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(_encryptionKey))
                throw new InvalidOperationException("Encryption key not set.");

            byte[] keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            byte[] iv = new byte[16];

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(encryptedText));
            using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }
    }
}
