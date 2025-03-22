using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Core.DTO;
using RxBlockChain.Model.Entities;
using System;
using RxBlockChain.Model;

namespace RxBlockChain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionDTO transaction)
        {
            if (transaction == null || !ModelState.IsValid)
            {
                var badResponse = ReturnedResponse<object>.ErrorResponse("Invalid transaction data. Please provide valid details.", null);
                return BadRequest(badResponse);
            }

            var response = await _transactionService.CreateTransactionAsync(transaction);
            return response.code == 200 ? Ok(response) : BadRequest(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var response = await _transactionService.GetAllTransactionsAsync();

            return response.data == null || !response.data.Any()
                ? NotFound(response)
                : Ok(response);
        }


        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionById(Guid transactionId)
        {
            if (transactionId == Guid.Empty)
            {
                return BadRequest("Transaction Id is required.");
            }

            var response = await _transactionService.GetTransactionByIdAsync(transactionId);
            return response.data == null ? NotFound(response) : Ok(response);
        }

        [HttpGet("wallet/{walletAddress}")]
        public async Task<IActionResult> GetTransactionsByWallet(string walletAddress)
        {
            if (string.IsNullOrWhiteSpace(walletAddress))
            {
                return BadRequest("Wallet address is required.");
            }

            var response = await _transactionService.GetTransactionsByWalletAsync(walletAddress);
            return response.data == null || !response.data.Any() ? NotFound(response) : Ok(response);
        }
        private string GenerateTransactionHash(string fromAddress, string toAddress, decimal amount, DateTime timestamp)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                string rawData = $"{fromAddress}{toAddress}{amount}{timestamp:yyyyMMddHHmmssfff}";
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                return Convert.ToBase64String(bytes);
            }
        }
    }


}

