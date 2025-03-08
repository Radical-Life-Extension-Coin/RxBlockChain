using Microsoft.Extensions.Logging;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Services
{
    public class SmartContractService : ISmartContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RalexVM _ralexVM;
        private readonly ILogger<SmartContractService> _logger;
        private const decimal TransactionFee = 1000m; // Fee in Ralex Coins

        public SmartContractService(IUnitOfWork unitOfWork, ILogger<SmartContractService> logger)
        {
            _unitOfWork = unitOfWork;
            _ralexVM = new RalexVM();
            _logger = logger;
        }

        public async Task<ApiResponse<SmartContract>> Deploy(SmartContract contract, string userAddress)
        {
            try
            {
                var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Address == userAddress);
                if (user == null || user.Balance < TransactionFee)
                    return ReturnedResponse<SmartContract>.ErrorResponse("Insufficient balance for deployment fee.", null);

                // Deduct fee and log transaction
                user.Balance -= TransactionFee;
                user.Transactions.Add(new Transactions
                {
                    Id = Guid.NewGuid(),
                    FromAddress = userAddress,
                    ToAddress = contract.ContractAddress,
                    Amount = TransactionFee,
                    TimeStamp = DateTime.UtcNow,
                    Type = "Deployment",
                    ContractAddress = contract.ContractAddress
                });

                 _unitOfWork.User.Update(user);
                contract.State = new Dictionary<string, object>(); // Initialize contract state
                await _unitOfWork.SmartContracts.AddAsync(contract);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Smart contract {contract.ContractAddress} deployed successfully by {userAddress}.");
                return ReturnedResponse<SmartContract>.SuccessResponse("Smart contract deployed successfully.", contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deploying smart contract.");
                return ReturnedResponse<SmartContract>.ErrorResponse("Deployment failed.", null);
            }
        }


        public async Task<ApiResponse<object>> Execute(string contractAddress, string function, object[] args, string userAddress)
        {
            try
            {
                var user = await _unitOfWork.User.GetFirstOrDefaultAsync(u => u.Address == userAddress);
                if (user == null || user.Balance < TransactionFee)
                    return ReturnedResponse<object>.ErrorResponse("Insufficient balance for execution fee.", null);

                // Deduct fee and log transaction
                user.Balance -= TransactionFee;
                user.Transactions.Add(new Transactions
                {
                    Id = Guid.NewGuid(),
                    FromAddress = userAddress,
                    ToAddress = contractAddress,
                    Amount = TransactionFee,
                    TimeStamp = DateTime.UtcNow,
                    Type = "Execution",
                    ContractAddress = contractAddress
                });

                 _unitOfWork.User.Update(user);

                var contract = await _unitOfWork.SmartContracts.GetFirstOrDefaultAsync(c => c.ContractAddress == contractAddress);
                if (contract == null)
                    return ReturnedResponse<object>.ErrorResponse("Smart contract not found.", null);

                var executionResult = await _ralexVM.ExecuteContract(contract, function, args);
                return executionResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Smart contract execution failed.");
                return ReturnedResponse<object>.ErrorResponse("Execution error.", null);
            }
        }

    }
}
