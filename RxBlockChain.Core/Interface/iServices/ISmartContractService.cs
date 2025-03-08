using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface ISmartContractService
    {
        Task<ApiResponse<SmartContract>> Deploy(SmartContract contract, string userAddress);
        Task<ApiResponse<object>> Execute(string contractAddress, string function, object[] args, string userAddress);
    }
}
