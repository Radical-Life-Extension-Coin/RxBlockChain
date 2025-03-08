using RxBlockChain.Model.Entities;
using RxBlockChain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using RxBlockChain.Core.DTO;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface IWalletService
    {

        Task<ApiResponse<WalletDTO>> CreateWalletAsync(); 
        Task<ApiResponse<Wallet>> GetWalletByAddressAsync(string address); 
        Task<ApiResponse<Wallet>> GetFirstOrDefaultAsync(string walletAddress);
        Task<ApiResponse<Wallet>> GetWalletByMnemonicAsync(string mnemonic); 
        Task<ApiResponse<WalletDTO>> GetOrCreateGenesisWalletAsync(); 


    }

}
