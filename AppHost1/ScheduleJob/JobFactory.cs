using ScheduleJob.Contractor;
using ScheduleJob.Enumn;
using ScheduleJob.JobExcutor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob
{
    public class JobFactory
    {
        public static TaskBase GetDefaultJob(TaskType type)
        {
            TaskBase task = null;
            switch (type)
            {
                case TaskType.SyncStock:
                    task = new SyncStockTask();
                    break;
                default:
                    return null;
            }
            return task.AssembleDefaultTask();
        }
    }
}
