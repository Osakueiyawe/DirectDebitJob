using DirectDebitJob.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Interfaces
{
    interface ISqlConnect
    {
        Task<DataTable> GetAllMandateRequests();
        Task<bool> UpdateUserMandateDetails(UpdateMandateDetails mandatedetails);
        Task<bool> UpdateTransactionLog(TransactionLogDetails transactiondetails);
        Task<DataTable> GetEnterpriseLifeApiDetails(string referencenumber, string basisReference);
        
    }
}
