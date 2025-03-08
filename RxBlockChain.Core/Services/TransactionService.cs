using NBitcoin;
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


        //public async Task<ApiResponse<Transactions>> CreateTransactionAsync(TransactionDTO transactionDto)
        //{
        //    string transactionHash = Guid.NewGuid().ToString();


        //    decimal fee = transactionDto.Amount * 0.01m;

        //    var senderWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == transactionDto.FromAddress);
        //    if (senderWallet == null)
        //    {
        //        return ReturnedResponse<Transactions>.ErrorResponse("Sender wallet not found.", null);
        //    }

        //    var recipientWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == transactionDto.ToAddress);
        //    if (recipientWallet == null)
        //    {
        //        return ReturnedResponse<Transactions>.ErrorResponse("Recipient wallet not found.", null);
        //    }

        //    if (senderWallet.Balance < (transactionDto.Amount + fee))
        //    {
        //        return ReturnedResponse<Transactions>.ErrorResponse("Insufficient funds.", null);
        //    }

        //    senderWallet.Balance -= (transactionDto.Amount + fee);
        //    recipientWallet.Balance += transactionDto.Amount;

        //    var messageToSign = transactionHash;
        //    byte[] signedTransaction = SignMessageWithPrivateKey(senderWallet.Mnemonic, messageToSign);

        //    var transaction = new Transactions
        //    {
        //        Id = Guid.NewGuid(),
        //        TransactionHash = transactionHash,
        //        Amount = transactionDto.Amount,
        //        Fee = fee,
        //        FromAddress = transactionDto.FromAddress,
        //        ToAddress = transactionDto.ToAddress,
        //        TimeStamp = DateTime.UtcNow,
        //        BlockId = null,
        //        Block = null,
        //        Signature = Convert.ToBase64String(signedTransaction)
        //    };

        //    await _unitOfWork.Transactions.AddAsync(transaction);
        //    await _unitOfWork.CompleteAsync();

        //    return ReturnedResponse<Transactions>.SuccessResponse("Transaction created successfully.", transaction);
        //  }

        public async Task<ApiResponse<Transactions>> CreateTransactionAsync(TransactionDTO transactionDto)
        {
            string transactionHash = Guid.NewGuid().ToString();
            decimal fee = transactionDto.Amount * 0.01m;

            var senderWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == transactionDto.FromAddress);
            if (senderWallet == null)
                return ReturnedResponse<Transactions>.ErrorResponse("Sender wallet not found.", null);

            var recipientWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == transactionDto.ToAddress);
            if (recipientWallet == null)
                return ReturnedResponse<Transactions>.ErrorResponse("Recipient wallet not found.", null);

            if (senderWallet.Balance < (transactionDto.Amount + fee))
                return ReturnedResponse<Transactions>.ErrorResponse("Insufficient funds.", null);

            if (string.IsNullOrWhiteSpace(senderWallet.Mnemonic) || !IsValidMnemonic(senderWallet.Mnemonic))
                return ReturnedResponse<Transactions>.ErrorResponse("Invalid mnemonic for sender wallet.", null);

            senderWallet.Balance -= (transactionDto.Amount + fee);
            recipientWallet.Balance += transactionDto.Amount;

            byte[] signedTransaction;
            try
            {
                signedTransaction = SignMessageWithPrivateKey(senderWallet.Mnemonic, transactionHash);
            }
            catch (Exception ex)
            {
                return ReturnedResponse<Transactions>.ErrorResponse("Transaction signing failed: " + ex.Message, null);
            }

            var transaction = new Transactions
            {
                Id = Guid.NewGuid(),
                TransactionHash = transactionHash,
                Amount = transactionDto.Amount,
                Fee = fee,
                FromAddress = transactionDto.FromAddress,
                ToAddress = transactionDto.ToAddress,
                TimeStamp = DateTime.UtcNow,
                BlockId = null,
                Block = null,
                Signature = Convert.ToBase64String(signedTransaction)
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.CompleteAsync();

            return ReturnedResponse<Transactions>.SuccessResponse("Transaction created successfully.", transaction);
        }

        public byte[] SignMessageWithPrivateKey(string mnemonic, string message)
        {
            if (string.IsNullOrWhiteSpace(mnemonic) || string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Mnemonic and message must not be empty.");

            try
            {
                var ecdsa = GetPrivateKeyFromECDsa(mnemonic);
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                return ecdsa.SignData(messageBytes, HashAlgorithmName.SHA256);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to sign the message with the private key.", ex);
            }
        }


        private bool IsValidMnemonic(string mnemonic)
        {
            if (string.IsNullOrWhiteSpace(mnemonic))
                return false;

            var words = mnemonic.Split(' ');
            return words.Length == 12 || words.Length == 15 || words.Length == 18 || words.Length == 21 || words.Length == 24;
        }

        private ECDsa GetPrivateKeyFromECDsa(string mnemonic)
        {
            try
            {
                if (!IsValidMnemonic(mnemonic))
                    throw new ArgumentException("Invalid mnemonic. The word count must be 12, 15, 18, 21, or 24.");

                var mnemonicObj = new Mnemonic(mnemonic, Wordlist.English);
                var extKey = mnemonicObj.DeriveExtKey();
                var privateKey = extKey.PrivateKey;

                byte[] privateKeyBytes = privateKey.ToBytes();

                using (ECDsa ecdsa = ECDsa.Create())
                {
                    ecdsa.ImportECPrivateKey(privateKeyBytes, out _);
                    return ecdsa;
                }
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

    }
}
