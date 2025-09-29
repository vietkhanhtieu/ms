using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.JobExcutor
{
    public interface IJobExcutor
    {
        System.Threading.Tasks.Task ExecutorAsync(TaskBase task);
    }
}
