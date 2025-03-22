using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Model.Entities;
using System.Security.Cryptography;
using System.Text;
using RxBlockChain.Core.DTO;
using RxBlockChain.Model;

namespace RxBlockChain.Core.Services
{
    public class BlockService : IBlockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;
        private readonly IWalletService _walletService;
        private readonly IValidatorStakingService _validatorService;


        public BlockService(IUnitOfWork unitOfWork, ITransactionService transactionService, IWalletService walletService, IValidatorStakingService validatorService)
        {
            _unitOfWork = unitOfWork;
            _transactionService = transactionService;
            _walletService = walletService;
             _validatorService = validatorService;
        }


       

       
        public async Task<ApiResponse<BlockDTO>> CreateBlockAsync()
        {
            
            var blocks = await _unitOfWork.Blocks.GetAllAsync();
            
            
            if (blocks == null)
            {
                return ReturnedResponse<BlockDTO>.ErrorResponse("No block in blockchain.", null);
            }

            var previousBlock = blocks.Last();

            var response = await _transactionService.GetPendingTransactions();
            if (response.code != 200) 
            {
                return ReturnedResponse<BlockDTO>.ErrorResponse("No pending transaction in mempool.", null);
            }

            List<Transactions> pendingTransactions = response.data.ToList();
            var validatorResponse = await _validatorService.SelectValidatorAsync();
            if(validatorResponse.code != 200)
            {
                return ReturnedResponse<BlockDTO>.ErrorResponse("No validators found.", null);
            }

            string validatorAddress = validatorResponse.data.WalletAddress;

           Block newBlock = new()
            {               
                BlockHeight = previousBlock.BlockHeight + 1,
                TimeStamp = DateTime.UtcNow,
                PreviousHash = previousBlock.PreviousHash,
                Transactions = pendingTransactions,
                ValidatorAddress = validatorAddress,
                MerkleRoot = ComputeMerkleRoot(pendingTransactions),
            };
            newBlock.BlockHash = ComputeBlockHash(newBlock);


            await _unitOfWork.Blocks.AddAsync(newBlock);
            await _unitOfWork.CompleteAsync();

            decimal fees = 0;

            foreach (var transaction in pendingTransactions)
            {
                transaction.ModifiedAt = DateTime.UtcNow;
                transaction.BlockId = newBlock.Id;
                fees += transaction.Fee;
            }

            var resp = await _walletService.GetWalletByAddressAsync(validatorAddress);
            if (resp.code==200)
            {
                var wallet = resp.data;
                wallet.Balance = wallet.Balance + fees;

            }
            await _unitOfWork.CompleteAsync();
            BlockDTO blockDTO = new()
            {
                Id = newBlock.Id,
                BlockHash = newBlock.BlockHash,
                BlockHeight = newBlock.BlockHeight,
                TimeStamp = newBlock.TimeStamp,
                Transactions = newBlock.Transactions,
            };
            

            return ReturnedResponse<BlockDTO>.SuccessResponse("Block created successfully.", blockDTO);
        }


      

        private string ComputeMerkleRoot(List<Transactions> transactions)
        {
            if (transactions == null || transactions.Count == 0)
                return string.Empty;

            // Start with each transaction's hash.
            List<string> hashes = transactions.Select(tx => tx.TransactionHash).ToList();

            // Continue reducing the list until one hash remains.
            while (hashes.Count > 1)
            {
                List<string> newHashes = new List<string>();
                for (int i = 0; i < hashes.Count; i += 2)
                {
                    // If there is an odd number, duplicate the last hash.
                    if (i + 1 < hashes.Count)
                    {
                        newHashes.Add(ComputeHash(hashes[i] + hashes[i + 1]));
                    }
                    else
                    {
                        newHashes.Add(ComputeHash(hashes[i] + hashes[i]));
                    }
                }
                hashes = newHashes;
            }
            return hashes[0];
        }

        private string ComputeBlockHash(Block block)
        {
            // Ensure that the Merkle root is computed. You might call ComputeMerkleRoot here if not already set.
            if (string.IsNullOrEmpty(block.MerkleRoot))
            {
                block.MerkleRoot = ComputeMerkleRoot(block.Transactions);
            }

            // Combine fields from the block header.
            string blockData = block.BlockHeight.ToString() +
                               block.TimeStamp.ToString("o") +  // ISO 8601 format
                               block.PreviousHash +
                               block.Nonce.ToString() +
                               block.MerkleRoot +
                               block.ValidatorAddress +         // For PoS, the validator that forged the block
                               block.Version.ToString();

            return ComputeHash(blockData);
        }

        private static string ComputeHash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }


        public async Task<Block> GetBlockByIdAsync(Guid blockId)
        {
            return await _unitOfWork.Blocks.FindSingleAsync(b => b.Id == blockId);
        }

        public async Task<Block> GetBlockByHeightAsync(int blockHeight)
        {
            return await _unitOfWork.Blocks.FindSingleAsync(b => b.BlockHeight == blockHeight);
        }

        public async Task<IEnumerable<Block>> GetAllBlocksAsync()
        {
            return await _unitOfWork.Blocks.GetAllAsync();
        }
    }
}
