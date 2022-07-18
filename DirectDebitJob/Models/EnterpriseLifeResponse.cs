using System;
using System.Collections.Generic;
using System.Text;

namespace DirectDebitJob.Models
{
    class EnterpriseLifeResponse
    {
        public PaymentUpdateResponse paymentupdateresponse { get; set; }
        public string hash { get; set; }
    }
    class PaymentUpdateResponse
    {
        public string referenceID { get; set; }
        public string transReference { get; set; }
        public string PaymentReference { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDesc { get; set; }
    }
}
