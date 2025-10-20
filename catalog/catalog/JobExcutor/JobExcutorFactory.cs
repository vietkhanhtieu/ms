using catalog.Enumerate;
using System;

namespace catalog.JobExcutor
{
    public sealed class JobExecutorFactory
    {
        private readonly IServiceProvider _sp;
        public JobExecutorFactory(IServiceProvider sp) => _sp = sp;

        public IJobExecutor GetTaskExecutor(JobType taskType) => taskType switch
        {
            JobType.SyncStock => _sp.GetRequiredService<SyncStockExecutor>(),
            JobType.EventJob => _sp.GetRequiredService<EventExecutor>(),
            _ => throw new NotSupportedException($"Not supported task: {taskType}")
        };
    }
}
