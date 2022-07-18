using DirectDebitJob.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DirectDebitJob
{
    public class Worker : BackgroundService
    {        
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDirectDebitProcess _directdebit;
        private readonly ISecurity _security;
        public Worker(ILogger<Worker> logger, IConfiguration configuration, IDirectDebitProcess directdebit, ISecurity security)
        {
            _logger = logger;
            _configuration = configuration;
            _directdebit = directdebit;            
            _security = security;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)        
        {                    
            while (!stoppingToken.IsCancellationRequested)
            {
                int jobinterval;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                bool isnumber = int.TryParse(_configuration.GetSection("Jobinterval").Value, out jobinterval);
                if (isnumber)
                {
                    await _directdebit.ProcessTransactions();
                    int delay = jobinterval * 60 * 60 * 1000;
                    _logger.LogInformation($"About to delay for the next : {jobinterval.ToString()} hours");
                    await Task.Delay(delay, stoppingToken);
                }
                else
                {
                    _logger.LogInformation($"Interval input must be a whole number");
                    _logger.LogError("Interval Input must be a whole number");
                    break;
                }

            }
            
            
        }
    }
}
