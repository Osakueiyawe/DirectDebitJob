using DirectDebitJob.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Services
{
    class DateUtility:IDateUtility
    {
        private readonly ILogger<DateUtility> _logger;
        public DateUtility(ILogger<DateUtility> logger)
        {
            _logger = logger;
        }
        public async Task<int> GetMonthInterval(DateTime firstdate, DateTime enddate, int frequency)
        {
            int interval = 0;
            try
            {
                int monthsApart = 12 * (firstdate.Year - enddate.Year) + firstdate.Month - enddate.Month;
                int monthdifference = Math.Abs(monthsApart);
                if (enddate.Day < firstdate.Day)
                {
                    monthdifference--;
                }
                interval = (int)(monthdifference / frequency);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting Month Interval ", ex);
            }
            return interval + 1;
        }
    }
}
