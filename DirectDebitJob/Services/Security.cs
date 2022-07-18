using DirectDebitJob.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Methods
{
    class Security:ISecurity
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Security> _logger;
        public Security(IConfiguration configuration, ILogger<Security> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> CheckIP()
        {
            bool result = false;
            try
            {
                string myHost = Dns.GetHostName();
                var host = Dns.GetHostByName(myHost);
                string myIP = "";
                foreach (var ab in host.AddressList)
                {
                    if (ab.AddressFamily.ToString() == "InterNetwork")
                    {
                        myIP = ab.ToString();                        
                    }
                }
                _logger.LogInformation($"IP is {myIP}");
                if (myIP == _configuration.GetSection("MachineIP").Value)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error verifying IP", ex);
            }
            return result;
        }
    }
}
