using NBitcoin;
using Newtonsoft.Json;
using RxBlockChain.Core.DTO;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;
using System.Security.Cryptography;
using System.Text;

namespace RxBlockChain.Core.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ApiResponse<Transactions>> CreateTransactionAsync(TransactionDTO transactionDto)
        {
            string transactionHash = Guid.NewGuid().ToString();
            decimal fee = transactionDto.Amount * 0.01m;

            var senderWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == transactionDto.FromAddress);
            if (senderWallet == null)
                return ReturnedResponse<Transactions>.ErrorResponse("Sender wallet not found.", null);

            var recipientWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == transactionDto.ToAddress);
            if (recipientWallet == null)
                return ReturnedResponse<Transactions>.ErrorResponse("Recipient wallet address not found.", null);

            if (senderWallet.Balance < (transactionDto.Amount + fee))
                return ReturnedResponse<Transactions>.ErrorResponse("Insufficient funds.", null);

            if (string.IsNullOrWhiteSpace(transactionDto.Mnemonic) || transactionDto.Mnemonic == "")
                return ReturnedResponse<Transactions>.ErrorResponse("Mnemonic for sender wallet not found.", null);

            

            var transaction = new Transactions
            {
                TransactionHash = transactionHash,
                Amount = transactionDto.Amount,
                Fee = fee,
                FromAddress = transactionDto.FromAddress,
                ToAddress = transactionDto.ToAddress,
                TimeStamp = DateTime.UtcNow,
                Signature = ""
            };

            try
            {
                transaction.Signature = SignTransactionToBase64(transactionDto.Mnemonic, transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inner Exception: " + ex.InnerException?.Message);
                return ReturnedResponse<Transactions>.ErrorResponse("Transaction signing failed: " + ex.Message, null);
            }


            senderWallet.Balance -= (transactionDto.Amount + fee);

            
            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.CompleteAsync();

            return ReturnedResponse<Transactions>.SuccessResponse("Transaction created successfully.", transaction);
        }

       
        public string SignTransactionToBase64(string mnemonic, Transactions transaction)
        {
            if (string.IsNullOrWhiteSpace(mnemonic))  throw new ArgumentException("Mnemonic must not be empty.", nameof(mnemonic));

            try
            {
                string serializedTransaction = JsonConvert.SerializeObject(transaction);
                byte[] transactionBytes = Encoding.UTF8.GetBytes(serializedTransaction);

                using var ecdsa = GetPrivateKeyFromECDsa(mnemonic);
                byte[] signature = ecdsa.SignData(transactionBytes, HashAlgorithmName.SHA256);
                return Convert.ToBase64String(signature);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to sign the transaction.", ex);
            }
        }

        

        private static bool IsValidMnemonic(string mnemonic)
        {
            if (string.IsNullOrWhiteSpace(mnemonic))
                return false;

            var words = mnemonic.Split(' ');
            return words.Length == 12 || words.Length == 15 || words.Length == 18 || words.Length == 21 || words.Length == 24;
        }

        private static ECDsa GetPrivateKeyFromECDsa(string mnemonic)
        {
            try
            {
                if (!IsValidMnemonic(mnemonic))
                    throw new ArgumentException("Invalid mnemonic. The word count must be 12, 15, 18, 21, or 24.");

                var mnemonicObj = new Mnemonic(mnemonic, Wordlist.English);
                var extKey = mnemonicObj.DeriveExtKey();
                var privateKey = extKey.PrivateKey;

                byte[] privateKeyBytes = privateKey.ToBytes();
                // byte[] derEncodedKey = EncodeECPrivateKey(rawKeyBytes);

                using ECDsa ecdsa = ECDsa.Create();
                ecdsa.ImportECPrivateKey(privateKeyBytes, out _);
                return ecdsa;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid mnemonic or wordlist issue.", ex);
            }
        }


        public async Task<ApiResponse<IEnumerable<Transactions>>> GetAllTransactionsAsync()
        {
         
            var transactions = await _unitOfWork.Transactions.GetAllAsync();


            if (transactions == null || !transactions.Any())
            {
                return ReturnedResponse<IEnumerable<Transactions>>.ErrorResponse("No transactions found.", null);
            }

          
            return ReturnedResponse<IEnumerable<Transactions>>.SuccessResponse("Transactions retrieved successfully.", transactions);
        }

        public async Task<ApiResponse<Transactions>> GetTransactionByIdAsync(Guid transactionId)
        {
            var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                return ReturnedResponse<Transactions>.ErrorResponse("Transaction not found.", null);
            }
            return ReturnedResponse<Transactions>.SuccessResponse("Transaction retrieved successfully.", transaction);
        }



        public async Task<ApiResponse<IEnumerable<Transactions>>> GetTransactionsByWalletAsync(string walletAddress)
        {
            var transactions = await _unitOfWork.Transactions.FindAsync(
                t => t.FromAddress == walletAddress || t.ToAddress == walletAddress);

            if (transactions == null || !transactions.Any())
            {
                return ReturnedResponse<IEnumerable<Transactions>>.ErrorResponse("No transactions found for this wallet.", null);
            }

            return ReturnedResponse<IEnumerable<Transactions>>.SuccessResponse("Transactions retrieved successfully.", transactions);
        }


        public async Task<ApiResponse<IEnumerable<Transactions>>> GetPendingTransactions()
        {
            var transactions = await _unitOfWork.Transactions.FindAsync(
                t => t.BlockId == Guid.Empty);

            if (transactions == null || !transactions.Any())
            {
                return ReturnedResponse<IEnumerable<Transactions>>.ErrorResponse("No transactions found for this wallet.", null);
            }

            return ReturnedResponse<IEnumerable<Transactions>>.SuccessResponse("Transactions retrieved successfully.", transactions);
        }

    }
}
