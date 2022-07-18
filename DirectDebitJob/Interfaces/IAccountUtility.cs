using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Interfaces
{
    interface IAccountUtility
    {
        Task<string> GetOldAccountfromFullAcctKey(string fullaccountkey);        
    }
}
