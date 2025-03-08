using RxBlockChain.Model.Entities;
using RxBlockChain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxBlockChain.Core.Services
{   
    public class RalexVM
    {
        public async Task<ApiResponse<object>> ExecuteContract(SmartContract contract, string function, object[] args)
        {
            try
            {
                // Validate contract state
                if (contract.State == null)
                    contract.State = new Dictionary<string, object>();

                object result;
                switch (function.ToLower())
                {
                    case "transfer":
                        result = ExecuteTransfer(contract, args);
                        break;
                    case "mint":
                        result = ExecuteMint(contract, args);
                        break;
                    case "burn":
                        result = ExecuteBurn(contract, args);
                        break;
                    default:
                        return ReturnedResponse<object>.ErrorResponse("Function not found in contract.", null);
                }

                // Simulate execution delay
                await Task.Delay(50);
                return ReturnedResponse<object>.SuccessResponse("Execution successful.", result);
            }
            catch (Exception ex)
            {
                return ReturnedResponse<object>.ErrorResponse($"Execution error: {ex.Message}", null);
            }
        }

        private object ExecuteTransfer(SmartContract contract, object[] args)
        {
            if (args.Length < 2) throw new ArgumentException("Invalid arguments for transfer.");
            var recipient = args[0].ToString();
            var amount = Convert.ToDecimal(args[1]);

            if (!contract.State.ContainsKey("balance"))
                contract.State["balance"] = 100000m; // Default balance for testing

            decimal currentBalance = (decimal)contract.State["balance"];
            if (currentBalance < amount)
                throw new Exception("Insufficient balance.");

            contract.State["balance"] = currentBalance - amount;
            return $"Transferred {amount} to {recipient}. New balance: {contract.State["balance"]}";
        }

        private object ExecuteMint(SmartContract contract, object[] args)
        {
            if (args.Length < 1) throw new ArgumentException("Invalid arguments for mint.");
            var amount = Convert.ToDecimal(args[0]);

            if (!contract.State.ContainsKey("balance"))
                contract.State["balance"] = 0m;

            contract.State["balance"] = (decimal)contract.State["balance"] + amount;
            return $"Minted {amount}. New balance: {contract.State["balance"]}";
        }

        private object ExecuteBurn(SmartContract contract, object[] args)
        {
            if (args.Length < 1) throw new ArgumentException("Invalid arguments for burn.");
            var amount = Convert.ToDecimal(args[0]);

            if (!contract.State.ContainsKey("balance"))
                contract.State["balance"] = 0m;

            decimal currentBalance = (decimal)contract.State["balance"];
            if (currentBalance < amount)
                throw new Exception("Insufficient balance to burn.");

            contract.State["balance"] = currentBalance - amount;
            return $"Burned {amount}. New balance: {contract.State["balance"]}";
        }
    }
}
