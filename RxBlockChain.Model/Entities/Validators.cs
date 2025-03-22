

namespace RxBlockChain.Model.Entities
{
    public class Validators : BaseEntity
    {
        public string WalletAddress { get; set; } = string.Empty;
        public decimal StakeAmount { get; set; }
        public DateTime? UnstakeRequestedAt { get; set; } 

        public bool IsActive { get; set; } = true; 

        public bool IsSlashed { get; set; } = false; 
    }
}
