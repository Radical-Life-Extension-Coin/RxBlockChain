using System.ComponentModel.DataAnnotations.Schema;

namespace RxBlockChain.Model.Entities
{
    public class Block : BaseEntity
    {
        public int BlockHeight { get; set; }
        public string PreviousHash { get; set; } = string.Empty;
        public string BlockHash { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;      
        public int Nonce { get; set; } 
        public List<Transactions> Transactions { get; set; } = new List<Transactions>();
        public string MerkleRoot { get; set; } = string.Empty;
        public string ValidatorAddress { get; set; } = string.Empty;
        public int Version { get; set; } = 1;
    }


}
