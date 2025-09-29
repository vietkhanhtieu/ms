using catalog.Contractor;
using catalog.Enumerate;

namespace catalog.JobExcutor
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _sp;
        public JobFactory(IServiceProvider sp) => _sp = sp;

        public JobBase GetTaskExecutor(JobType taskType) => taskType switch
        {
            JobType.SyncStock => _sp.GetRequiredService<SyncStockJob>(),
            _ => throw new NotSupportedException($"Not supported task: {taskType}")
        };
    }
}
