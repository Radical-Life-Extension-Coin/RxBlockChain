using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.DTO
{
    public class ValidatorDTO
    {
        public string Message { get; set; }
        public String WalletAddress { get; set; }

        public decimal StakeAmount { get; set; }

    }
}
