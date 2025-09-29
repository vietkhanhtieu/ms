using catalog.Enumerate;
using catalog.JobExcutor;
using catalog.Models;
using System.Threading.Tasks;

namespace catalog.Contractor
{
    public class SyncStockJob : JobBase
    {
        public override JobBase AssembleDefaultTask()
        {
            return new SyncStockJob()
            {
                Id = GenerateId(),
                JobSchedule = new JobSchedule()
                {
                    Id = GenerateId(),
                    Interval = 30,
                    JobInternalType = JobInternalType.Seconds
                },
                NextRunTime = DateTime.UtcNow.Ticks,
                CreatedAt = DateTime.UtcNow,
                Type = JobType.SyncStock,
            };
        }
    }
}
