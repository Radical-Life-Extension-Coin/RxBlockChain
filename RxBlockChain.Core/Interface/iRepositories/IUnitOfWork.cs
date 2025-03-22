using System;
using System.Threading.Tasks;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<Transactions> Transactions { get; }
        IGenericRepository<Block> Blocks { get; }
        IGenericRepository<SmartContract> SmartContracts { get; }
        IGenericRepository<Node> Nodes { get; }
        IGenericRepository<User> Users { get; }
        IGenericRepository<Validators> Validators { get; }
        Task<int> CompleteAsync();
    }
}
