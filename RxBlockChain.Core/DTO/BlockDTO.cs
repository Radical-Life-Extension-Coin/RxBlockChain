using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.DTO
{
    public class BlockDTO
    {
        public Guid Id { get; set; }
        public string BlockHash { get; set; }
        public int BlockHeight { get; set; }
        public DateTime TimeStamp { get; set; }
        public List<Transactions> Transactions { get; set; }
    }
}
