using ScheduleJob.Enumn;
using ScheduleJob.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = ScheduleJob.Enumn.TaskStatus;

namespace ScheduleJob.JobExcutor
{
    public abstract class TaskBase
    {
        [Key]
        public string Id { get; set; }
        public TaskType Type { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long NextRunTime { get; set; }
        public TaskSchedule TaskSchedule { get; set; }
        public DateTime? CaculateNextRunTime(long nextRuntime = -1)
        {
            var time = nextRuntime > 0 ? nextRuntime : NextRunTime;
            var nextRunDay = new DateTime(time, DateTimeKind.Utc);
            var interval = TaskSchedule?.Interval ?? 0;
            var intervalType = TaskSchedule?.TaskInternalType ?? TaskInternalType.Minutes;
            switch (intervalType)
            {
                case TaskInternalType.OnlyOnce:
                    return DateTime.MaxValue;
                case TaskInternalType.Seconds:
                    return nextRunDay.AddSeconds(interval);
                case TaskInternalType.Minutes:
                    return nextRunDay.AddMinutes(interval);
                case TaskInternalType.Hours:
                    return nextRunDay.AddHours(interval);
                case TaskInternalType.Daily:
                    return nextRunDay.AddDays(interval);
                case TaskInternalType.Weekly:
                    return nextRunDay.AddDays(7 * interval);
                case TaskInternalType.Monthly:
                    return nextRunDay.AddMonths(interval);
                default:
                    return null;
            }
        }

        protected string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        public abstract TaskBase AssembleDefaultTask();
    }
}
