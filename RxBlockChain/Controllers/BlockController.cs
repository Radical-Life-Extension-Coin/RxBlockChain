using Microsoft.AspNetCore.Mvc;
using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockController : ControllerBase
    {
        private readonly IBlockService _blockService;

        public BlockController(IBlockService blockService)
        {
            _blockService = blockService;
        }

        /// <summary>
        /// Creates a new block.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateBlock([FromBody] Block block)
        {
            if (block == null)
                return BadRequest("Invalid block data.");

            try
            {
                var createdBlock = await _blockService.CreateBlockAsync(block);
                return Ok(createdBlock);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a block by its Id.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlockById(Guid id)
        {
            var block = await _blockService.GetBlockByIdAsync(id);
            if (block == null)
                return NotFound("Block not found.");
            return Ok(block);
        }

        /// <summary>
        /// Retrieves a block by its height.
        /// </summary>
        [HttpGet("height/{height}")]
        public async Task<IActionResult> GetBlockByHeight(int height)
        {
            var block = await _blockService.GetBlockByHeightAsync(height);
            if (block == null)
                return NotFound("Block not found.");
            return Ok(block);
        }

        /// <summary>
        /// Retrieves all blocks.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBlocks()
        {
            var blocks = await _blockService.GetAllBlocksAsync();
            return Ok(blocks);
        }
    }
}
