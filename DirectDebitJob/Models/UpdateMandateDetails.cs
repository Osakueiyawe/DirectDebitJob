using System;
using System.Collections.Generic;
using System.Text;

namespace DirectDebitJob.Models
{
    class UpdateMandateDetails
    {
        public string referencenumber { get; set; }
        public DateTime? nextpaymentdate { get; set; }
        public DateTime lastPaymentDate { get; set; }
        public decimal lastPaymentAmount { get; set; }
    }
}
