using ScheduleJob.Enumn;
using ScheduleJob.JobExcutor;
using ScheduleJob.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.Contractor
{
    public class SyncStockTask : TaskBase
    {
        public override TaskBase AssembleDefaultTask()
        {
            return new SyncStockExcutor()
            {
                Id = GenerateId(),
                TaskSchedule = new TaskSchedule()
                {
                    Id = GenerateId(),
                    Interval = 30,
                    TaskInternalType = TaskInternalType.Seconds
                },
                NextRunTime = DateTime.UtcNow.Ticks,
                CreatedAt = DateTime.UtcNow,
                Type = TaskType.SyncStock,
            };
        }
    }
}
