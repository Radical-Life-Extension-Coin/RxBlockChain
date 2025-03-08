namespace RxBlockChain.Model.Entities
{
    public class User
    {
   
            public string Id { get; set; }  // Unique identifier
            public string Address { get; set; } // Blockchain wallet address
            public decimal Balance { get; set; } // Ralex Coin balance
            public List<Transactions> Transactions { get; set; } = new List<Transactions>(); // User's transactions
        }
}
