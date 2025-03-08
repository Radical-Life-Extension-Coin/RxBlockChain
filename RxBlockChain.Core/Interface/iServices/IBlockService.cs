using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface IBlockService
    {
        /// <summary>
        /// Creates a new block.
        /// </summary>
        Task<Block> CreateBlockAsync(Block block);

        /// <summary>
        /// Retrieves a block by its Id.
        /// </summary>
        Task<Block> GetBlockByIdAsync(Guid blockId);

        /// <summary>
        /// Retrieves a block by its block height.
        /// </summary>
        Task<Block> GetBlockByHeightAsync(int blockHeight);

        /// <summary>
        /// Retrieves all blocks.
        /// </summary>
        Task<IEnumerable<Block>> GetAllBlocksAsync();
    }
}
