

using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.Interface.iServices;

namespace RxBlockChain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidatorController : ControllerBase
    {
        private readonly IValidatorStakingService _validatorService;

        public ValidatorController(IValidatorStakingService validatorService)
        {
            _validatorService = validatorService;
        }


        [HttpPost("stake")]
        public async Task<IActionResult> Stake(string wallet, decimal amount) =>
       Ok(await _validatorService.StakeAsync(wallet, amount));


        [HttpPost("unstake")]
        public async Task<IActionResult> RequestUnstake(string wallet) =>
       Ok(await _validatorService.RequestUnstakeAsync(wallet));


        [HttpGet("select-validator")]
        public async Task<IActionResult> SelectValidator() =>
        Ok(await _validatorService.SelectValidatorAsync());

        [HttpGet("view-validator")]
        public async Task<IActionResult> ViewValidators() =>
        Ok(await _validatorService.ViewValidatorAsync());


        [HttpPost("slash")]
        public async Task<IActionResult> Slash(string wallet) =>
        Ok(await _validatorService.SlashValidatorAsync(wallet));
    }
}

