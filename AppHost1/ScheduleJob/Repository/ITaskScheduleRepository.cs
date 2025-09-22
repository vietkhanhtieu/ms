using ScheduleJob.JobExcutor;
using ScheduleJob.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.Repository
{
    public interface ITaskScheduleRepository
    {
        Task<List<Job>> GetAllTasks();
        Task<bool> ReleaseTask(string id);
        Task<bool> CreateTask(TaskBase job);

    }
}
