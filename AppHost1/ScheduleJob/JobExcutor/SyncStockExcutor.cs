using Microsoft.Extensions.Logging;
using ScheduleJob.Enumn;
using ScheduleJob.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleJob.JobExcutor
{
    public class SyncStockExcutor : IJobExcutor
    {

        private readonly ILogger<SyncStockExcutor> _logger;

        public SyncStockExcutor(ILogger<SyncStockExcutor> logger)
            => _logger = logger;

        public async System.Threading.Tasks.Task ExecutorAsync(TaskBase task)
        {
            _logger.LogInformation($"Starting stock synchronization for Task ID: {task.Id}");
        }
    }
}
