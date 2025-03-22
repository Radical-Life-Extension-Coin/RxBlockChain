
using RxBlockChain.Model.Entities;
namespace RxBlockChain.Core.Interface.iServices
{
    public interface IProofOfStake
    {
        public Validators SelectValidator(List<Validators> Validators);
    }

}
