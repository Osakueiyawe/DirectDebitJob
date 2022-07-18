using System;
using System.Collections.Generic;
using System.Text;

namespace DirectDebitJob.Models
{
    class EnterpriseLifeRequest
    {
        public string paymentReference { get; set; }
        public string sessionID { get; set; }
        public string debitBankCode { get; set; }
        public string debitAccountNumber { get; set; }
        public string debitAccountName { get; set; }
        public string creditBankCode { get; set; }
        public string creditAccountNumber { get; set; }
        public string creditAccountName { get; set; }
        public string transactionAmount { get; set; }
        public string transactionDate { get; set; }
        public string currency { get; set; }
    }    
}
