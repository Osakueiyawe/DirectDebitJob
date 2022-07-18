using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Interfaces
{
    interface IDateUtility
    {
        Task<int> GetMonthInterval(DateTime firstdate, DateTime enddate, int frequency);
    }
}
