using NBitcoin;
using RxBlockChain.Model.Entities;

namespace RxBlockChain.Core.Interface.iRepositories
{
    public interface ISignRepository
    {
        Task<string> GetPrivateKeyFromWalletAsync(WalletKeyId keyId);
    }
}
