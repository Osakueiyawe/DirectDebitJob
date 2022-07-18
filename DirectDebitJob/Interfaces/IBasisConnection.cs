using DirectDebitJob.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Interfaces
{
    interface IBasisConnection
    {
        Task<string> PostTransactionDetails(PostTransactionDetails transactiondetails);
    }
}
