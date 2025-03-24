

using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.Interface.iServices;

namespace RxBlockChain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

     
        [HttpPost("create")]
        public async Task<IActionResult> CreateWallet()
        {
            var response = await _walletService.CreateWalletAsync();
            return response.code == 200 ? Ok(response) : BadRequest(response);
        }

        [HttpGet()]
        public async Task<IActionResult> GetWalletByAddress(string walletAddress)
        {
            if (string.IsNullOrEmpty(walletAddress))
            {
                return BadRequest("Wallet address is required.");
            }

            var response = await _walletService.GetWalletByAddressAsync(walletAddress);
            return response.code == 200 ? Ok(response) : NotFound(response);
        }

       
        [HttpPost("retrieveWalletByPhrase")]
        public async Task<IActionResult> GetWalletByMnemonic([FromBody] string mnemonicPhrase)
        {
            if (string.IsNullOrEmpty(mnemonicPhrase))
            {
                return BadRequest("Mnemonic is required.");
            }

            var response = await _walletService.GetWalletByMnemonicAsync(mnemonicPhrase);
            return response.code == 200 ? Ok(response) : NotFound(response);
        }


        [HttpGet("genesis")]
        public async Task<IActionResult> GetOrCreateGenesisWallet()
        {
            var response = await _walletService.GetOrCreateGenesisWalletAsync();
            return response.code == 200 ? Ok(response) : BadRequest(response);
        }
    }
}

