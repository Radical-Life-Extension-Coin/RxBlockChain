using RxBlockChain.Core.DTO;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model.Entities;
using RxBlockChain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace RxBlockChain.Core.Services
{
    public class ValidatorStakingService : IValidatorStakingService
    {
        private const decimal MIN_STAKE = 10m;
        private const int UNSTAKE_COOLDOWN = 2; 
        private readonly IUnitOfWork _unitOfWork;

        public ValidatorStakingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<ValidatorDTO>> StakeAsync(string walletAddress, decimal amount)
        {
            if (amount < MIN_STAKE)
            return ReturnedResponse<ValidatorDTO>.ErrorResponse("Minimum stake is 10 tokens.", null);

            var response = await _unitOfWork.Validators.FindSingleAsync(v => v.WalletAddress == walletAddress);

            if (response != null) return ReturnedResponse<ValidatorDTO>.ErrorResponse("Validator already exist.", null);

            var wresponse = await _unitOfWork.Wallets.FindSingleAsync(v => v.Address == walletAddress);
            if (wresponse == null) return ReturnedResponse<ValidatorDTO>.ErrorResponse("Invalid validator Wallet Address.", null);

            Validators validator = new()
            {
                WalletAddress=walletAddress,
                StakeAmount = amount,
               IsActive = true
            };
                

            ValidatorDTO validatorDTO = new()
            {
                Message = "Validator has successfully staked",
                WalletAddress = validator.WalletAddress
            };

            await _unitOfWork.Validators.AddAsync(validator);
            await _unitOfWork.CompleteAsync();

            return ReturnedResponse<ValidatorDTO>.SuccessResponse($"Validator has successfully staked  {amount}", validatorDTO);
        }

        public async Task<ApiResponse<ValidatorDTO>> RequestUnstakeAsync(string walletAddress)
        {
            var validator = await _unitOfWork.Validators.FindSingleAsync(v => v.WalletAddress == walletAddress);

            if (validator == null || validator.StakeAmount == 0)
            return ReturnedResponse<ValidatorDTO>.ErrorResponse("No active stake found.", null);

            validator.UnstakeRequestedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            ValidatorDTO validatorDTO = new()
            {
                Message = $"Unstaking requested. You can withdraw in {UNSTAKE_COOLDOWN} days.",
                WalletAddress = validator.WalletAddress
            };

            return ReturnedResponse<ValidatorDTO>.SuccessResponse($"Unstaking requested. You can withdraw in {UNSTAKE_COOLDOWN} days.", validatorDTO);

        }

        // 🟡 Process Unstaking After Cooldown
        public async Task<ApiResponse<ValidatorDTO>> CompleteUnstakeAsync(string walletAddress)
        {
            var validator = await _unitOfWork.Validators.FindSingleAsync(v => v.WalletAddress == walletAddress);

            if (validator == null || validator.UnstakeRequestedAt == null)
            return ReturnedResponse<ValidatorDTO>.ErrorResponse("No unstaking request found.", null);

            if ((DateTime.UtcNow - validator.UnstakeRequestedAt.Value).TotalDays < UNSTAKE_COOLDOWN)
            return ReturnedResponse<ValidatorDTO>.ErrorResponse("Unstaking cooldown not yet complete.", null);

            decimal unstakedAmount = validator.StakeAmount;
            validator.StakeAmount = 0;
            validator.IsActive = false;
            validator.UnstakeRequestedAt = null;

            await _unitOfWork.CompleteAsync();
            ValidatorDTO validatorDTO = new()
            {
                Message = $"Successfully unstaked {unstakedAmount} tokens!",
                WalletAddress = validator.WalletAddress
            };
            return ReturnedResponse<ValidatorDTO>.SuccessResponse($"Successfully unstaked {unstakedAmount} tokens!", validatorDTO);

        }

        // 🏆 Select a Validator for Block Creation (Weighted Random)
       public async Task<ApiResponse<ValidatorDTO>> SelectValidatorAsync()
        {
            var validators = await _unitOfWork.Validators.FindAsync(v => v.IsActive && !v.IsSlashed);
            if (!validators.Any()) return ReturnedResponse<ValidatorDTO>.ErrorResponse("No active validators available.", null); 

            var totalStake = validators.Sum(v => v.StakeAmount);
            var randomValue = new Random().NextDouble() * (double)totalStake;
            

            decimal cumulative = 0;
            foreach (var validator in validators)
            {
                cumulative += validator.StakeAmount;
                if ((decimal)randomValue < cumulative)
                {
                    ValidatorDTO validatorDTO = new()
                    {
                        Message = $"Selected Validator: {validator.WalletAddress}",
                        WalletAddress = validator.WalletAddress
                    }; 
                    return  ReturnedResponse<ValidatorDTO>.SuccessResponse($"Selected Validator: {validator.WalletAddress}", validatorDTO);
                }
            }
           
            return ReturnedResponse<ValidatorDTO>.ErrorResponse("Validator selection failed.", null);
        }

        public async Task<ApiResponse<List<ValidatorDTO>>> ViewValidatorAsync()
        {
            var validators = await _unitOfWork.Validators.FindAsync(v => v.IsActive && !v.IsSlashed);
            if (!validators.Any()) return ReturnedResponse<List<ValidatorDTO>>.ErrorResponse("No active validators available.", null);

            List<ValidatorDTO> validatorDTOs = new();

            foreach (var validator in validators)
            {                
                    ValidatorDTO validatorDTO = new()
                    {
                        Message = "Validator Found",
                        WalletAddress = validator.WalletAddress,
                        StakeAmount = validator.StakeAmount
                    };
                validatorDTOs.Add(validatorDTO);
            }
            return ReturnedResponse<List<ValidatorDTO>>.SuccessResponse($"{validatorDTOs.Count} Validators found .", validatorDTOs);
        }



        // 🎁 Distribute Rewards
        public async Task<ApiResponse<string>> DistributeRewardsAsync()
        {
            var validators = await _unitOfWork.Validators.FindAsync(v => v.IsActive && !v.IsSlashed);
            if (!validators.Any()) return ReturnedResponse<string>.ErrorResponse("No active validators to reward.", "No active validators to reward");

            var totalStake = validators.Sum(v => v.StakeAmount);
            foreach (var validator in validators)
            {
                decimal reward = (validator.StakeAmount / totalStake) * 100;
                validator.StakeAmount += reward;
            }

            await _unitOfWork.CompleteAsync();
            return ReturnedResponse<string>.SuccessResponse($"Distributed {100} tokens among active validators.", $"Distributed {100} tokens among active validators.");
            
        }

        // ⚠️ Slash Validator (For Misbehavior)
        public async Task<string> SlashValidatorAsync(string walletAddress)
        {
            var validator = await _unitOfWork.Validators.FindSingleAsync(v => v.WalletAddress == walletAddress);
            if (validator == null) return "Validator not found.";

            decimal penalty = validator.StakeAmount * 0.5m; // 50% penalty
            validator.StakeAmount -= penalty;
            validator.IsSlashed = true;
            validator.IsActive = false;

            await _unitOfWork.CompleteAsync();
            return $"Validator {walletAddress} slashed by {penalty} tokens.";
        }
    }
}
