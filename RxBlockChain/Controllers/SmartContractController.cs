using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.DTO;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/contracts")]
public class SmartContractController : ControllerBase
{
    private readonly ISmartContractService _smartContractService;

    public SmartContractController(ISmartContractService smartContractService)
    {
        _smartContractService = smartContractService;
    }

    /// <summary>
    /// Deploy a new smart contract.
    /// </summary>
    /// <param name="contract">SmartContract object</param>
    /// <returns>Deployment result</returns>
    [HttpPost("deploy")]
    public async Task<ActionResult<ApiResponse<SmartContract>>> Deploy([FromBody] SmartContract contract)
    {
        if (contract == null)
            return BadRequest(ReturnedResponse<SmartContract>.ErrorResponse("Invalid contract data.", null));

        try
        {
            // Extract user address from headers
            string userAddress = Request.Headers["User-Address"].ToString();
            if (string.IsNullOrEmpty(userAddress))
                return Unauthorized(ReturnedResponse<SmartContract>.ErrorResponse("User address is required in headers.", null));

            var response = await _smartContractService.Deploy(contract, userAddress);
            return response.code == 200 ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ReturnedResponse<SmartContract>.ErrorResponse("Internal server error.", null));
        }
    }

    /// <summary>
    /// Execute a function on a smart contract.
    /// </summary>
    /// <param name="request">Smart contract execution request</param>
    /// <returns>Execution result</returns>
    [HttpPost("execute")]
    public async Task<ActionResult<ApiResponse<object>>> Execute([FromBody] SmartContractDTO request)
    {
        if (request == null || string.IsNullOrEmpty(request.ContractAddress) || string.IsNullOrEmpty(request.Function))
            return BadRequest(ReturnedResponse<object>.ErrorResponse("Invalid execution request.", null));

        try
        {
            // Extract user address from headers
            string userAddress = Request.Headers["User-Address"].ToString();
            if (string.IsNullOrEmpty(userAddress))
                return Unauthorized(ReturnedResponse<object>.ErrorResponse("User address is required in headers.", null));

            var response = await _smartContractService.Execute(request.ContractAddress, request.Function, request.Args, userAddress);
            return response.code == 200 ? Ok(response) : BadRequest(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ReturnedResponse<object>.ErrorResponse("Internal server error.", null));
        }
    }
}
