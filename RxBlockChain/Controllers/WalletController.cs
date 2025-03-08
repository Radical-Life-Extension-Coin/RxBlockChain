

using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Core.DTO;

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
            return response.code == "200" ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{address}")]
        public async Task<IActionResult> GetWalletByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return BadRequest("Wallet address is required.");
            }

            var response = await _walletService.GetWalletByAddressAsync(address);
            return response.code == "200" ? Ok(response) : NotFound(response);
        }

       
        [HttpPost("retrieveWalletByPhrase")]
        public async Task<IActionResult> GetWalletByMnemonic([FromBody] string mnemonic)
        {
            if (string.IsNullOrEmpty(mnemonic))
            {
                return BadRequest("Mnemonic is required.");
            }

            var response = await _walletService.GetWalletByMnemonicAsync(mnemonic);
            return response.code == "200" ? Ok(response) : NotFound(response);
        }


        [HttpGet("genesis")]
        public async Task<IActionResult> GetOrCreateGenesisWallet()
        {
            var response = await _walletService.GetOrCreateGenesisWalletAsync();
            return response.code == "200" ? Ok(response) : BadRequest(response);
        }
    }
}

