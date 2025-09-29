using catalog.Enumerate;
using catalog.JobExcutor;
using catalog.Models;
using catalog.Repository.Interfaces;

namespace catalog.Repository.Implementations
{
    public class JobRepository : IJobRepository
    {
        private readonly ILogger<JobRepository> _logger;
        private readonly CatalogContext _context;
        private List<JobType> excludeTasks = new List<JobType>() { };
        private readonly IJobFactory _jobFactory;


        public JobRepository(ILogger<JobRepository> logger, CatalogContext context, IJobFactory jobFactory)
        {
            _logger = logger;
            _context = context;
            _jobFactory = jobFactory;
        }

        public async Task InitTask()
        {
            try
            {
                var taskTypes = Enum.GetValues(typeof(JobType));
                foreach (var taskType in taskTypes)
                {
                    var type = (JobType)taskType;
                    if (!excludeTasks.Contains(type))
                    {
                        var job = _jobFactory.GetTaskExecutor(type);
                        await CreateTask(job);
                    }
                    else
                    {
                        _logger.LogInformation($"skip to run job: {type}");
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error initializing tasks: {ex.Message}");
            }
        }

        public async Task<bool> CreateTask(JobBase taskBase)
        {
            try
            {
                var job = ConvertJobBaseToJob(taskBase);
                if (!_context.Jobs.Any(t => t.Type == job.Type))
                {
                    await _context.Jobs.AddAsync(job);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public Task<List<Job>> GetAllTasks()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReleaseTask(string id)
        {
            throw new NotImplementedException();
        }


        private Job ConvertJobBaseToJob(JobBase job)
        {
            var taskSchedule = job.JobSchedule != null
                ? new JobSchedule
                {
                    Id = Guid.NewGuid().ToString(),
                    JobId = job.Id,
                    Interval = job.JobSchedule.Interval,
                    JobInternalType = job.JobSchedule.JobInternalType
                }
                : null;

            return new Job
            {
                Id = job.Id,
                Status = job.Status,
                CreatedAt = job.CreatedAt,
                StartedAt = job.StartedAt,
                Type = job.Type,
                NextRunTime = job.NextRunTime,
                JobSchedule = taskSchedule
            };
        }
    }
}
