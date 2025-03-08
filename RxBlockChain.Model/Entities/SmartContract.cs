
namespace RxBlockChain.Model.Entities
{
    public class SmartContract
    {
        

            public string ContractAddress { get; set; } = Guid.NewGuid().ToString();
       
            public string Code { get; set; } 
            public string OwnerAddress { get; set; } 
            public bool IsActive { get; set; } = true;
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> State { get; set; }

        public SmartContract(string code, string ownerAddress)
            {
                Code = code;
                OwnerAddress = ownerAddress;
            }
        

    }
}
