using System.ComponentModel.DataAnnotations.Schema;

namespace RxBlockChain.Model.Entities
{
    public class Block
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int BlockHeight { get; set; }
        public string BlockHash { get; set; } = string.Empty;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string PreviousHash { get; set; } = string.Empty;
        public int Nonce { get; set; } 
        public List<Transactions> Transactions { get; set; } = new List<Transactions>();
            
        [NotMapped]
        public object MerkleRoot { get; set; }
        public List<SmartContract> SmartContracts { get; set; }
        [NotMapped]
        public object Validator { get; set; }
    }

}
