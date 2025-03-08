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
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<WalletDTO>> CreateWalletAsync()
        {
            try
            {
                // Generate 24-word mnemonic
                var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
                string mnemonicWords = mnemonic.ToString();

                // Derive private key from mnemonic
                string privateKey = GetPrivateKeyFromMnemonic(mnemonicWords);

                // Generate wallet address
                string walletAddress = GenerateWalletAddress(mnemonicWords);

                // Create the wallet object
                var wallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    Address = walletAddress,
                    Balance = 0m,
                    IsGenesis = false,
                    Mnemonic = mnemonicWords // You may choose to store the mnemonic or handle it securely
                };

                // Save wallet to DB
                await _unitOfWork.Wallets.AddAsync(wallet);
                await _unitOfWork.CompleteAsync();

                var responseData = new WalletDTO { Mnemonic = mnemonicWords, Wallet = wallet };

                return ReturnedResponse<WalletDTO>
                            .SuccessResponse("Wallet created successfully", responseData);
            }
            catch (Exception)
            {
                return ReturnedResponse<WalletDTO>
                            .ErrorResponse("Error creating wallet", null);
            }
        }

        private string GetPrivateKeyFromMnemonic(string mnemonic)
        {
            var mnemonicObj = new Mnemonic(mnemonic);
            var extKey = mnemonicObj.DeriveExtKey();
            var privateKey = extKey.PrivateKey;
            return privateKey.ToString();
        }


        public async Task<ApiResponse<Wallet>> GetWalletByAddressAsync(string address)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == address);
                if (wallet == null)
                {
                    return ReturnedResponse<Wallet>.ErrorResponse("Wallet not found", null);
                }
                return ReturnedResponse<Wallet>.SuccessResponse("Wallet retrieved successfully", wallet);
            }
            catch (Exception)
            {
                return ReturnedResponse<Wallet>.ErrorResponse("Error retrieving wallet", null);
            }
        }

        public async Task<ApiResponse<Wallet>> GetFirstOrDefaultAsync(string walletAddress)
        {
            try
            {
                var wallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == walletAddress);
                if (wallet == null)
                {
                    return ReturnedResponse<Wallet>.ErrorResponse("Wallet not found", null);
                }
                return ReturnedResponse<Wallet>.SuccessResponse("Wallet retrieved successfully", wallet);
            }
            catch (Exception)
            {
                return ReturnedResponse<Wallet>.ErrorResponse("Error retrieving wallet", null);
            }
        }

        public async Task<ApiResponse<Wallet>> GetWalletByMnemonicAsync(string mnemonic)
        {
            try
            {
                string walletAddress = GenerateWalletAddress(mnemonic);
                var wallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == walletAddress);
                if (wallet == null)
                {
                    return ReturnedResponse<Wallet>.ErrorResponse("Wallet not found", null);
                }
                return ReturnedResponse<Wallet>.SuccessResponse("Wallet retrieved successfully", wallet);
            }
            catch (Exception)
            {
                return ReturnedResponse<Wallet>.ErrorResponse("Error retrieving wallet by mnemonic", null);
            }
        }

        public async Task<ApiResponse<WalletDTO>> GetOrCreateGenesisWalletAsync()
        {
            try
            {
                var existingGenesisWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.IsGenesis);
                if (existingGenesisWallet != null)
                {
                    return ReturnedResponse<WalletDTO>
                                .SuccessResponse("Genesis wallet retrieved successfully", new WalletDTO { Mnemonic = null, Wallet = existingGenesisWallet });
                }

                var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
                string mnemonicWords = mnemonic.ToString();
                string walletAddress = GenerateWalletAddress(mnemonicWords);

                var genesisWallet = new Wallet
                {
                    Id = Guid.NewGuid(),
                    Address = walletAddress,
                    Balance = 1000000000m, // 1 billion coins
                    IsGenesis = true,
                    Mnemonic = mnemonicWords // Store the mnemonic securely or as per your security standards
                };

                await _unitOfWork.Wallets.AddAsync(genesisWallet);
                await _unitOfWork.CompleteAsync();

                var responseData = new WalletDTO { Mnemonic = mnemonicWords, Wallet = genesisWallet };

                return ReturnedResponse<WalletDTO>
                            .SuccessResponse("Genesis wallet created successfully", responseData);
            }
            catch (Exception)
            {
                return ReturnedResponse<WalletDTO>
                            .ErrorResponse("Error retrieving or creating genesis wallet", null);
            }
        }

        private string GenerateWalletAddress(string mnemonic)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(mnemonic);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

       
    }
}

