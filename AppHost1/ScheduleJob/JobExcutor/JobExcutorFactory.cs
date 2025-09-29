using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.JobExcutor
{
    public class JobExcutorFactory
    {
        public static IJobExcutor GetJobExcutor(Enumn.TaskType type)
        {
            IJobExcutor jobExcutor = null;
            //switch (type)
            //{
            //    case Enumn.TaskType.SyncStock:
            //        jobExcutor = new SyncStockExcutor();
            //        break;
            //    default:
            //        return null;
            //}
            return jobExcutor;
        }
    }
}
