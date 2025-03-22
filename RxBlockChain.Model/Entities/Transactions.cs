
using RxBlockChain.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RxBlockChain.Model.Entities
{
    public class Transactions : BaseEntity
    {

        [Required]
        public string TransactionHash { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        [Required] 
        public string FromAddress { get; set; } = string.Empty;

        [Required] 
        public string ToAddress { get; set; } = string.Empty;

        public string Signature { get; set; } = string.Empty;
        public string ContractAddress { get; set; } = string.Empty;
        public TransactionType Type { get; set; }

        public decimal Fee { get; set; } 

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public Guid BlockId { get; set; }

    }
}

