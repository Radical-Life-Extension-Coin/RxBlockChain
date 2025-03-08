using RxBlockChain.Data.Database;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Data.Repositories;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlockChainDb _context;
        private IGenericRepository<Wallet> _wallets;
        private IGenericRepository<Transactions> _transactions;
        private IGenericRepository<Block> _blocks;
        private IGenericRepository<SmartContract> _smartContracts;
        private IGenericRepository<Node> _nodes;
        private IGenericRepository<User> _user;
        //private IGenericRepository<PosConsensus> _posConsensus;
       // private IGenericRepository<WalletKeyId> _walletKeyIds;

        public UnitOfWork(BlockChainDb context)
        {
            _context = context;
        }

        public IGenericRepository<Wallet> Wallets => _wallets ??= new GenericRepository<Wallet>(_context);
        public IGenericRepository<Transactions> Transactions => _transactions ??= new GenericRepository<Transactions>(_context);
        public IGenericRepository<Block> Blocks => _blocks ??= new GenericRepository<Block>(_context);
        public IGenericRepository<SmartContract> SmartContracts => _smartContracts ??= new GenericRepository<SmartContract>(_context);
        public IGenericRepository<Node> Nodes => _nodes ??= new GenericRepository<Node>(_context);
        public IGenericRepository<User> User => _user ??= new GenericRepository<User>(_context);
       // public IGenericRepository<PosConsensus> PosConsensus => _posConsensus ??= new GenericRepository<PosConsensus>(_context);
        //public IGenericRepository<WalletKeyId> WalletKeyIds => _walletKeyIds ??= new GenericRepository<WalletKeyId>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
