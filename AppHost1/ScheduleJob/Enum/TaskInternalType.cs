using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.Enumn
{
    public enum TaskInternalType
    {
        OnlyOnce = 1,
        Seconds = 2,
        Minutes = 3,
        Hours = 4,
        Daily = 5,
        Weekly = 6,
        Monthly = 7
    }
}
