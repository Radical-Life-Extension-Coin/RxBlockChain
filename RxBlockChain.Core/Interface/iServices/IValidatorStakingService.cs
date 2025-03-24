using RxBlockChain.Model;
using RxBlockChain.Core.DTO;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface IValidatorStakingService
    {
        Task<ApiResponse<ValidatorDTO>> StakeAsync(string walletAddress, decimal amount);
        Task<ApiResponse<ValidatorDTO>> RequestUnstakeAsync(string walletAddress);
        Task<ApiResponse<ValidatorDTO>> CompleteUnstakeAsync(string walletAddress);
        Task<ApiResponse<ValidatorDTO>> SelectValidatorAsync();
        Task<ApiResponse<string>> DistributeRewardsAsync();
        Task<string> SlashValidatorAsync(string walletAddress);
        Task<ApiResponse<List<ValidatorDTO>>> ViewValidatorAsync();
    }

}
