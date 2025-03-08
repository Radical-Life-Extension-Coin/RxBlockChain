using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxBlockChain.Model.Entities
{
    public class PosConsensus
    {
  
            private readonly List<Node> _nodes;

            public PosConsensus(List<Node> nodes)
            {
                _nodes = nodes;
            }

            public Node SelectValidator()
            {
                decimal totalStake = _nodes.Sum(n => n.StakeAmount);
                decimal randomValue = new Random().Next(0, (int)totalStake);

                decimal cumulativeStake = 0;
                foreach (var node in _nodes.OrderBy(n => Guid.NewGuid())) 
                {
                    cumulativeStake += node.StakeAmount;
                    if (randomValue <= cumulativeStake)
                        return node;
                }

                return _nodes.First(); 
            }
        

    }
}
