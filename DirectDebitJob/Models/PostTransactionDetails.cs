using System;
using System.Collections.Generic;
using System.Text;

namespace DirectDebitJob.Models
{
    class PostTransactionDetails
    {
        public string accountfrom { get; set; }
        public string accountto { get; set; }
        public double transactionamount { get; set; }
        public int expl_code { get; set; }
        public string remarks { get; set; }
        public string reqst_code { get; set; }
        public int man_app1 { get; set; }
        public int tellid { get; set; }
        public string docalpha { get; set; }
        public int originatingbracode { get; set; }
    }
}
