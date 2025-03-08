using System.Threading.Tasks;
using RxBlockChain.Core.Interface.iRepositories;
using RxBlockChain.Model.Entities;
using System.Linq;
using NBitcoin;

namespace RxBlockChain.Data.Repositories
{
    public class SignRepository : ISignRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public SignRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GetPrivateKeyFromWalletAsync(WalletKeyId keyId)
        {
            // Retrieve the wallet using the UnitOfWork
            var wallet = await _unitOfWork.Wallets.GetFirstOrDefaultAsync(w => w.Address == keyId.ToString());

            if (wallet == null)
            {
                throw new KeyNotFoundException("Wallet not found for the given keyId.");
            }

            // For the sake of example, we're returning a placeholder private key.
            // Replace with actual logic for private key retrieval.
            return "PrivateKeyPlaceholder";
        }
    }
}
