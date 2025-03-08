using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.DTO;
using RxBlockChain.Core.Interface.iServices;

[ApiController]
[Route("api/nodes")]
public class NodeController : ControllerBase
{
    private readonly INodeService _nodeService;

    public NodeController(INodeService nodeService)
    {
        _nodeService = nodeService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterNode([FromBody] NodeDTO request)
    {
        var response = await _nodeService.RegisterNode(request.WalletAddress, request.StakeAmount);
        return Ok(response);
    }

    [HttpGet("peers")]
    public async Task<IActionResult> GetPeers()
    {
        var response = await _nodeService.GetPeers();
        return Ok(response);
    }

    [HttpGet("select-validator")]
    public async Task<IActionResult> SelectValidator()
    {
        var response = await _nodeService.SelectValidator();
        return Ok(response);
    }
}


