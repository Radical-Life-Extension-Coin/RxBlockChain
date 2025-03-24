using NBitcoin;
using RxBlockChain.Common.Helper;
using RxBlockChain.Common.Helper.Interface;
using RxBlockChain.Core.DTO;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidatorStakingService _validatorService;
        private readonly IAesEncryptionHelper _aesEncryptionHelper;

        public WalletService(IUnitOfWork unitOfWork, IValidatorStakingService validatorService, IAesEncryptionHelper aesEncryptionHelper)
        {
            _unitOfWork = unitOfWork;
            _validatorService = validatorService;
            _aesEncryptionHelper = aesEncryptionHelper;
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
                string encryptedPrivateKey = _aesEncryptionHelper.Encrypt(privateKey);

                // Generate wallet address
                string walletAddress = GenerateWalletAddress(mnemonicWords);

                var wallet = new Wallet
                {
                    Address = walletAddress,
                    Balance = 0m,
                    PrivateKey = encryptedPrivateKey
                };

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
                wallet.PrivateKey = _aesEncryptionHelper.Decrypt(wallet.PrivateKey);
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
                wallet.PrivateKey = _aesEncryptionHelper.Decrypt(wallet.PrivateKey);
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
               
                string privateKey = GetPrivateKeyFromMnemonic(mnemonicWords);
                string encryptedPrivateKey = _aesEncryptionHelper.Encrypt(privateKey);

                var genesisWallet = new Wallet
                {
                    Address = walletAddress,
                    Balance = 1000000000m, // 1 billion coins
                    IsGenesis = true,
                    IsValidator = true,
                    PrivateKey = encryptedPrivateKey
                };

                await _unitOfWork.Wallets.AddAsync(genesisWallet);

                decimal stakeAmount = 1000;

                var response = await _validatorService.StakeAsync(walletAddress, stakeAmount);                
                                
                genesisWallet.Balance -= stakeAmount;

                await _unitOfWork.CompleteAsync();

                var responseData = new WalletDTO { Mnemonic = mnemonicWords, Wallet = genesisWallet };

                return ReturnedResponse<WalletDTO>.SuccessResponse("Genesis wallet created successfully", responseData);
            }
            catch (Exception ex)
            {
                return ReturnedResponse<WalletDTO>.ErrorResponse($"Error retrieving or creating genesis wallet. {ex.Message}", null);
            }
        }
       

        private static string GenerateWalletAddress(string mnemonicPhrase)
        {
            var mnemonic = new Mnemonic(mnemonicPhrase, Wordlist.English);
            var masterKey = mnemonic.DeriveExtKey();
            var key = masterKey.Derive(0, true);

            var address = key.PrivateKey.PubKey.GetAddress(ScriptPubKeyType.Segwit, Network.Main).ToString();
            return address;
        }

        //public async Task<decimal> GetWalletBalance(string walletAddress)
        //{
        //    var received = await _unitOfWork.Transactions
        //        .GetFirstOrDefaultAsync(t => t.ToAddress == walletAddress)
        //        .SumAsync(t => t.Amount);

        //    var sent = await _unitOfWork.Transactions
        //        .GetFirstOrDefaultAsync(t => t.FromAddress == walletAddress)
        //        .SumAsync(t => t.Amount);

        //    return received - sent;
        //}



    }
}

