namespace RxBlockChain.Model.Entities
{
    public class Wallet
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }
        public bool IsValidator => Balance >= 10000000;
        public bool IsGenesis { get; set; }

        public string Mnemonic { get; set; }

        public string PrivateKey { get; internal set; }
    }
}
