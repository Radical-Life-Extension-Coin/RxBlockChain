using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.DTO
{
    public class WalletResponseDTO
    {
        public string Mnemonic { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Balance { get; set; }

    }
}
