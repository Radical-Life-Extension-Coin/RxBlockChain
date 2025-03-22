using RxBlockChain.Core.DTO;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface ITransactionService
    {
 
        Task<ApiResponse<Transactions>> CreateTransactionAsync(TransactionDTO transactionDTO);

        Task<ApiResponse<Transactions>> GetTransactionByIdAsync(Guid transactionId);
        Task<ApiResponse<IEnumerable<Transactions>>> GetTransactionsByWalletAsync(string walletAddress);
        Task<ApiResponse<IEnumerable<Transactions>>> GetAllTransactionsAsync();

        Task<ApiResponse<IEnumerable<Transactions>>> GetPendingTransactions();
    }
}
