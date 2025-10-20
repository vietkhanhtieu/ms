namespace catalog.JobExcutor
{
    public class SyncStockExecutor : IJobExecutor
    {
        private readonly ILogger<SyncStockExecutor> _logger;

        public SyncStockExecutor(ILogger<SyncStockExecutor> logger)
            => _logger = logger;

        public async System.Threading.Tasks.Task ExecutorAsync(JobBase task)
        {
            _logger.LogInformation($"Starting stock synchronization for Task ID: {task.Id}");
        }
    }
}
