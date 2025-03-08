using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.DTO
{
    public class WalletDTO
    {
        public string Mnemonic { get; set; }
        public Wallet Wallet { get; set; }
        public string PrivateKey { get; internal set; }
    }
}
