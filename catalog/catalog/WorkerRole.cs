using catalog.Enumerate;
using catalog.JobExcutor;
using catalog.Repository.Interfaces;
using System.Threading.Tasks;

namespace catalog
{
    public class WorkerRole : BackgroundService
    {
        private readonly ILogger<WorkerRole> _logger;
        private List<JobType> excludeTasks = new List<JobType>() { };
        private int _executionCount;
        private readonly IServiceScopeFactory _scopeFactory;
        //private readonly JobExecutorFactory _jobExcutor;


        public WorkerRole(ILogger<WorkerRole> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using var scope = _scopeFactory.CreateScope();
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                    var jobs = await jobRepository.GetAllTasks();
                    foreach(var job in jobs)
                    {
                        _logger.LogInformation($"Job: {job.Id}, Type: {job.Type}, Status: {job.Status}");
                        await DoWork(job, scope.ServiceProvider);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        private async Task DoWork(JobBase job, IServiceProvider serviceProvider)
        {
            _logger.LogInformation($"Start to process job: {job.Type}");
            try
            {
                var executorFactory = serviceProvider.GetRequiredService<JobExecutorFactory>();
                var executor = executorFactory.GetTaskExecutor(job.Type);
                await executor.ExecutorAsync(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing job: {job.Id}, job type: {job.Type}, message: {ex.Message}");
            }
        }
    }
}
