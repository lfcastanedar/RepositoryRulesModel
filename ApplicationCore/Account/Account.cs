using Infrastructure.Ethereum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Account
{
    public class Account
    {
        private SmartContractContext _smartContractContext;

        public Account(Infrastructure.Ethereum.SmartContractContext smartContractContext) { 
            _smartContractContext= smartContractContext;
        }
    }
}
