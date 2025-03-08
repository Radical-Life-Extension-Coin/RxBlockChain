using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface INodeService
    {
        Task<ApiResponse<Node>> RegisterNode(string walletAddress, decimal stakeAmount);
        Task<ApiResponse<List<string>>> GetPeers();
        Task<ApiResponse<Node>> SelectValidator();
    }
}
