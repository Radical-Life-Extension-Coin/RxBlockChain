namespace RxBlockChain.Common.Helper.Interface
{
    public interface IAesEncryptionHelper
    {
        public string Encrypt(string plainText);
        public string Decrypt(string encryptedText);
    }
}
