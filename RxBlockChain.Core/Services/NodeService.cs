using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Services
{
    public class NodeService : INodeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NodeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Node>> RegisterNode(string walletAddress, decimal stakeAmount)
        {
            var existingNode = await _unitOfWork.Nodes.GetFirstOrDefaultAsync(n => n.WalletAddress == walletAddress);
            if (existingNode != null)
            {
                return ReturnedResponse<Node>.ErrorResponse("Node already registered.", null);
            }

            var newNode = new Node
            {
                Id = Guid.NewGuid(),
                WalletAddress = walletAddress,
                StakeAmount = stakeAmount,
                IsValidator = false
            };

            await _unitOfWork.Nodes.AddAsync(newNode);
            await _unitOfWork.CompleteAsync();

            return ReturnedResponse<Node>.SuccessResponse("Node registered successfully.", newNode);
        }

        public async Task<ApiResponse<List<string>>> GetPeers()
        {
            var nodes = await _unitOfWork.Nodes.GetAllAsync();
            var peers = nodes.Select(n => n.WalletAddress).ToList();

            return peers.Count > 0
                ? ReturnedResponse<List<string>>.SuccessResponse("Peers retrieved successfully.", peers)
                : ReturnedResponse<List<string>>.ErrorResponse("No peers available.", null);
        }

        public async Task<ApiResponse<Node>> SelectValidator()
        {
            var nodes = await _unitOfWork.Nodes.GetAllAsync();
            if (!nodes.Any()) return ReturnedResponse<Node>.ErrorResponse("No nodes available.", null);

            decimal totalStake = nodes.Sum(n => n.StakeAmount);
            decimal randomValue = new Random().Next(0, (int)totalStake);

            decimal cumulativeStake = 0;
            foreach (var node in nodes.OrderBy(n => Guid.NewGuid())) // Shuffle for fairness
            {
                cumulativeStake += node.StakeAmount;
                if (randomValue <= cumulativeStake)
                {
                    node.IsValidator = true;
                    await _unitOfWork.CompleteAsync();
                    return ReturnedResponse<Node>.SuccessResponse("Validator selected.", node);
                }
            }

            return ReturnedResponse<Node>.ErrorResponse("No validator selected.", null);
        }

        public class PeerToPeerService
        {
            private readonly List<string> _peers = new List<string>(); // Store peer addresses

            public void AddPeer(string peerAddress)
            {
                if (!_peers.Contains(peerAddress))
                    _peers.Add(peerAddress);
            }

            public void BroadcastTransaction(Transactions transaction)
            {
                foreach (var peer in _peers)
                {
                    // Simulate sending transaction to each peer (WebSocket or HTTP)
                    Console.WriteLine($"Broadcasting transaction {transaction.TransactionHash} to {peer}");
                }
            }
        }

    }

}
