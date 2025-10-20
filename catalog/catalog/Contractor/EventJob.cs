using catalog.Enumerate;
using catalog.JobExcutor;
using catalog.Models;

namespace catalog.Contractor
{
    public class EventJob : JobBase
    {
        public override JobBase AssembleDefaultTask()
        {
            return new SyncStockJob()
            {
                Id = GenerateId(),
                JobSchedule = new JobSchedule()
                {
                    Id = GenerateId(),
                    Interval = 3,
                    JobInternalType = JobInternalType.Seconds
                },
                NextRunTime = DateTime.UtcNow.Ticks,
                CreatedAt = DateTime.UtcNow,
                Type = JobType.EventJob,
            };
        }
    }
}
