using RxBlockChain.Core.DTO;
using RxBlockChain.Model;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iServices
{
    public interface IBlockService
    {

        Task<ApiResponse<BlockDTO>> CreateBlockAsync();

        Task<Block> GetBlockByIdAsync(Guid blockId);
        
        Task<Block> GetBlockByHeightAsync(int blockHeight);
        
        Task<IEnumerable<Block>> GetAllBlocksAsync();
    }
}
