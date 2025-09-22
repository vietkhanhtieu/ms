using Microsoft.EntityFrameworkCore;
using ScheduleJob.JobExcutor;
using ScheduleJob.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.Repository
{
    public class TaskScheduleRepository : ITaskScheduleRepository
    {
        private readonly ScheduleDbContext _context;
        public TaskScheduleRepository(ScheduleDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> GetAllTasks()
        {
            return await _context.Jobs.ToListAsync();
        }

        public async Task<bool> CreateTask(TaskBase taskBase)
        {
            try
            {
                var job = ConvertTaskBaseToJob(taskBase);
                if (!_context.Jobs.Any(t => t.Type == job.Type))
                {
                    await _context.Jobs.AddAsync(job);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> ReleaseTask(string id)
        {
            throw new NotImplementedException();
        }

        private Job ConvertTaskBaseToJob(TaskBase job)
        {
            var taskSchedule = job.TaskSchedule != null
                ? new TaskSchedule
                {
                    Id =  Guid.NewGuid().ToString(),
                    TaskId = job.Id,
                    Interval = job.TaskSchedule.Interval,
                    TaskInternalType = job.TaskSchedule.TaskInternalType
                }
                : null;

            return new Job
            {
                Id = job.Id,
                Status = job.Status,
                CreatedAt = job.CreatedAt,
                StartedAt = job.StartedAt,
                Type = job.Type,
                NextRunTime = job.NextRunTime,
                TaskSchedule = taskSchedule
            };
        }

    }
}
