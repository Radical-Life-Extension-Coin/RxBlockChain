using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RxBlockChain.Model.Entities
{
    public class BlockChain
    {
        private readonly List<Block> _chain;

        public BlockChain() {
            _chain = new List<Block>();
        }
            public void AddBlock(Block newBlock)
            {
                if (_chain.Count > 0)
                    newBlock.PreviousHash = _chain.Last().BlockHash;

                newBlock.BlockHash = GenerateHash(newBlock);
                _chain.Add(newBlock);
            }

            private string GenerateHash(Block block)
            {
                return Convert.ToBase64String(
                    SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(block.PreviousHash + block.TimeStamp))
                );
            }
        

    }
}
