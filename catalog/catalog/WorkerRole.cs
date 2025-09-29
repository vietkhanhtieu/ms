using catalog.Enumerate;
using catalog.JobExcutor;
using catalog.Repository.Interfaces;

namespace catalog
{
    public class WorkerRole : BackgroundService
    {
        private readonly ILogger<WorkerRole> _logger;
        private List<JobType> excludeTasks = new List<JobType>() { };
        private IJobFactory _jobFactory;
        private int _executionCount;
        private readonly IServiceScopeFactory _scopeFactory;


        public WorkerRole(ILogger<WorkerRole> logger, IServiceScopeFactory scopeFactory, IJobFactory jobFactory)
        {
            _logger = logger;
            _jobFactory = jobFactory;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            // When the timer should have no due-time, then do the work once now.
            await DoWork();

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(30));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await DoWork();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        private async Task DoWork()
        {
            int count = Interlocked.Increment(ref _executionCount);

            // Simulate work
            await Task.Delay(TimeSpan.FromSeconds(2));

            _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("WorkerRole is running.");
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing worker, message: {ex.Message}");
            }
        }

        public async Task Process()
        {

        }

        
    }
}
