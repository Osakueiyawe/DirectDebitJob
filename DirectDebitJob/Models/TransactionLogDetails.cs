using System;
using System.Collections.Generic;
using System.Text;

namespace DirectDebitJob.Models
{
    class TransactionLogDetails
    {
        public string referencenumber { get; set; }
        public DateTime transactiondate { get; set; }
        public decimal amount { get; set; }
        public string basisreference { get; set; }
    }
}
