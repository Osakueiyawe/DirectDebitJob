using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DirectDebitJob.Interfaces
{
    public interface ISecurity
    {
        Task<bool> CheckIP();
    }
}
