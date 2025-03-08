using System;
using System.Collections.Generic;

namespace RxBlockChain.Model.Entities
{
    public class Node
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string WalletAddress { get; set; }
        public decimal StakeAmount { get; set; } = 0m; // PoS staking balance
        public bool IsValidator { get; set; } = false; // Indicates if the node is selected as a validator
        public List<string> Peers { get; set; } = new(); // List of connected peer nodes

        public Node() { }

        public Node(string walletAddress, decimal stakeAmount)
        {
            WalletAddress = walletAddress;
            StakeAmount = stakeAmount;
        }
    }
}
