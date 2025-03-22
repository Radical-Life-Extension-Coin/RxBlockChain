using RxBlockChain.Model.Entities;
using RxBlockChain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using RxBlockChain.Core.DTO;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface IWalletService
    {

        Task<ApiResponse<WalletResponseDTO>> CreateWalletAsync(); 
        Task<ApiResponse<Wallet>> GetWalletByAddressAsync(string walletAddress); 
        Task<ApiResponse<Wallet>> GetWalletByMnemonicAsync(string mnemonicPhrase); 
        Task<ApiResponse<WalletDTO>> GetOrCreateGenesisWalletAsync(); 


    }

}
