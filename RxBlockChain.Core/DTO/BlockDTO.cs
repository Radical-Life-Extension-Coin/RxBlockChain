using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxBlockChain.Core.DTO
{
    public class BlockDTO
    {
        public Guid Id { get; set; }
        public string BlockHash { get; set; }
        public DateTime TimeStamp { get; set; }
        public ICollection<TransactionDTO> Transactions { get; set; }
    }
}
