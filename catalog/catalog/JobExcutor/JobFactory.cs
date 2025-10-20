using catalog.Contractor;
using catalog.Enumerate;

namespace catalog.JobExcutor
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _sp;
        public JobFactory(IServiceProvider sp) => _sp = sp;

        public JobBase GetTaskExecutor(JobType jobType)
        {
            JobBase task = null;
            switch (jobType)
            {
                case JobType.SyncStock:
                    task = _sp.GetRequiredService<SyncStockJob>();
                    break;
                case JobType.EventJob:
                    task = _sp.GetRequiredService<EventJob>();
                    break;
                default:
                    return null;
            }
            return task.AssembleDefaultTask();
        }
    }
}
