using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxBlockChain.Core.DTO
{
    public class SmartContractDTO
    {
        public string ContractAddress { get; set; }
        public string Function { get; set; }
        public object[] Args { get; set; }
    }
}
