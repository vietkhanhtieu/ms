using catalog.Enumerate;
using catalog.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace catalog.JobExcutor
{
    public abstract class JobBase
    {
        public string Id { get; set; }
        public JobType Type { get; set; }
        public JobStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long NextRunTime { get; set; }
        public JobSchedule JobSchedule { get; set; }
        public DateTime? CaculateNextRunTime(long nextRuntime = -1)
        {
            var time = nextRuntime > 0 ? nextRuntime : NextRunTime;
            var nextRunDay = new DateTime(time, DateTimeKind.Utc);
            var interval = JobSchedule?.Interval ?? 0;
            var intervalType = JobSchedule?.JobInternalType ?? JobInternalType.Minutes;
            switch (intervalType)
            {
                case JobInternalType.OnlyOnce:
                    return DateTime.MaxValue;
                case JobInternalType.Seconds:
                    return nextRunDay.AddSeconds(interval);
                case JobInternalType.Minutes:
                    return nextRunDay.AddMinutes(interval);
                case JobInternalType.Hours:
                    return nextRunDay.AddHours(interval);
                case JobInternalType.Daily:
                    return nextRunDay.AddDays(interval);
                case JobInternalType.Weekly:
                    return nextRunDay.AddDays(7 * interval);
                case JobInternalType.Monthly:
                    return nextRunDay.AddMonths(interval);
                default:
                    return null;
            }
        }

        protected string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        public abstract JobBase AssembleDefaultTask();
    }
}
