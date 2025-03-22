using NBitcoin;
using RxBlockChain.Core.DTO;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;
using System.Security.Cryptography;
using System.Text;
using RxBlockChain.Utility;

namespace RxBlockChain.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidatorStakingService _validatorService;

        public WalletService(IUnitOfWork unitOfWork, IValidatorStakingService validatorService)
        {
            _unitOfWork = unitOfWork;
            _validatorService = validatorService;
        }

        public async Task<ApiResponse<WalletResponseDTO>> CreateWalletAsync()
        {
            try
            {
                var existingGenesisWallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.IsGenesis);
                if (existingGenesisWallet == null)
                {
                    return ReturnedResponse<WalletResponseDTO>.ErrorResponse("Genesis wallet not found. Please create Genesis Wallet First", null);
                }

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
                    Address = walletAddress,
                    Balance = 0m,
                    PrivateKey = privateKey
                };

                // Save wallet to DB
                await _unitOfWork.Wallets.AddAsync(wallet);
                await _unitOfWork.CompleteAsync();

                var responseData = new WalletResponseDTO { Mnemonic = mnemonicWords, Address = walletAddress, Balance = wallet.Balance };

                return ReturnedResponse<WalletResponseDTO>
                            .SuccessResponse("Wallet created successfully", responseData);
            }
            catch (Exception)
            {
                return ReturnedResponse<WalletResponseDTO>
                            .ErrorResponse("Error creating wallet", null);
            }
        }

        private static string GetPrivateKeyFromMnemonic(string mnemonic)
        {
            return new Mnemonic(mnemonic).DeriveExtKey().PrivateKey.ToHex();
        }


        public async Task<ApiResponse<Wallet>> GetWalletByAddressAsync(string walletAddress)
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

       

        public async Task<ApiResponse<Wallet>> GetWalletByMnemonicAsync(string mnemonicPhrase)
        {
            try
            {
                string walletAddress = GenerateWalletAddress(mnemonicPhrase);
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
                    return ReturnedResponse<WalletDTO>.SuccessResponse("Genesis wallet retrieved successfully", new WalletDTO { Mnemonic = null, Wallet = existingGenesisWallet });
                }

                var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
                string mnemonicWords = mnemonic.ToString();
                string walletAddress = GenerateWalletAddress(mnemonicWords);

                ExtKey extKey = mnemonic.DeriveExtKey();
                string privateKey = extKey.PrivateKey.ToHex();

                var genesisWallet = new Wallet
                {
                    Address = walletAddress,
                    Balance = 1000000000m, // 1 billion coins
                    IsGenesis = true,
                    IsValidator = true,
                    PrivateKey = privateKey
                };

                await _unitOfWork.Wallets.AddAsync(genesisWallet);
                await _unitOfWork.CompleteAsync();

                decimal stakeAmount = 1000;

                var response = await _validatorService.StakeAsync(walletAddress, stakeAmount);
                
                decimal bal = genesisWallet.Balance - stakeAmount;
                
                genesisWallet.Balance = bal;

                await _unitOfWork.CompleteAsync();

                var responseData = new WalletDTO { Mnemonic = mnemonicWords, Wallet = genesisWallet };

                return ReturnedResponse<WalletDTO>.SuccessResponse("Genesis wallet created successfully", responseData);
            }
            catch (Exception ex)
            {
                return ReturnedResponse<WalletDTO>.ErrorResponse($"Error retrieving or creating genesis wallet. {ex.Message}", null);
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

