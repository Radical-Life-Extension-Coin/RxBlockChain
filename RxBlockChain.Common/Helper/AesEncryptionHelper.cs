using Microsoft.Extensions.Options;
using RxBlockChain.Common.Helper.Interface;
using RxBlockChain.Model;
using System.Security.Cryptography;
using System.Text;

namespace RxBlockChain.Common.Helper
{
    public class AesEncryptionHelper  : IAesEncryptionHelper
    {
        private readonly AppSettings _settings;
        public AesEncryptionHelper( IOptions<AppSettings> settings)
        {
            _settings = settings.Value;
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(_settings.AesEncryptionKey))
                throw new InvalidOperationException("Encryption key not set.");

            byte[] keyBytes = Encoding.UTF8.GetBytes(_settings.AesEncryptionKey);
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

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(_settings.AesEncryptionKey))
                throw new InvalidOperationException("Encryption key not set.");

            byte[] keyBytes = Encoding.UTF8.GetBytes(_settings.AesEncryptionKey);
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
