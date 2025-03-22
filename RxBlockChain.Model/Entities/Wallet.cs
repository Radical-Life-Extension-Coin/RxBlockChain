namespace RxBlockChain.Model.Entities
{
    public class Wallet : BaseEntity
    {
        public string Address { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsValidator { get; set; } = false;
        public bool IsGenesis { get; set; } = false;
        public string PrivateKey { get; set; } = string.Empty;
    }
}
