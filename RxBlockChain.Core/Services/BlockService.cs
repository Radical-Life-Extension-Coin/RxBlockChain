using RxBlockChain.Core.Interface.iServices;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Services
{
    public class BlockService : IBlockService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BlockService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Block> CreateBlockAsync(Block block)
        {
            block.Id = Guid.NewGuid();
            block.TimeStamp = DateTime.UtcNow;

            // You might want to assign BlockHeight here based on your chain logic.
            await _unitOfWork.Blocks.AddAsync(block);
            await _unitOfWork.CompleteAsync();

            return block;
        }

        public async Task<Block> GetBlockByIdAsync(Guid blockId)
        {
            return await _unitOfWork.Blocks.FindSingleAsync(b => b.Id == blockId);
        }

        public async Task<Block> GetBlockByHeightAsync(int blockHeight)
        {
            return await _unitOfWork.Blocks.FindSingleAsync(b => b.BlockHeight == blockHeight);
        }

        public async Task<IEnumerable<Block>> GetAllBlocksAsync()
        {
            return await _unitOfWork.Blocks.GetAllAsync();
        }
    }
}
