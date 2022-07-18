using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DirectDebitJob.Interfaces;
using Microsoft.Extensions.Logging;

namespace DirectDebitJob.Methods
{
    class AccountUtility:IAccountUtility
    {
        private readonly ILogger<AccountUtility> _logger;
        public AccountUtility(ILogger<AccountUtility> logger)
        {
            _logger = logger;
        }
        public async Task<string> GetOldAccountfromFullAcctKey(string fullaccountkey)
        {
            string bracode = "";
            string cusnum = "";
            string ledcode = "";
            string curcode = "";
            string subacctcode = "";
            string result = "";
            try
            {
                if (fullaccountkey.Length == 11)
                {
                    bracode = fullaccountkey.Substring(0, 3);
                    cusnum = fullaccountkey.Substring(3, 6);
                    curcode = fullaccountkey.Substring(9, 1);
                    ledcode = fullaccountkey.Substring(10, 1);
                    subacctcode = "0";
                }
                else if (fullaccountkey.Length == 12)
                {
                    bracode = fullaccountkey.Substring(0, 3);
                    cusnum = fullaccountkey.Substring(3, 6);
                    curcode = fullaccountkey.Substring(9, 1);
                    ledcode = fullaccountkey.Substring(10, 1);
                    subacctcode = fullaccountkey.Substring(11, 1);
                }
                else if (fullaccountkey.Length == 13)
                {
                    bracode = fullaccountkey.Substring(0, 3);
                    cusnum = fullaccountkey.Substring(3, 7);
                    curcode = fullaccountkey.Substring(10, 1);
                    ledcode = fullaccountkey.Substring(11, 1);
                    subacctcode = fullaccountkey.Substring(12, 1);
                }
                result = bracode + "/" + cusnum + "/" + curcode + "/" + ledcode + "/" + subacctcode;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error converting to Old Account",ex);
            }
            return result;
        }
        
    }
}
