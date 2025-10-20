using catalog.Enumerate;
using catalog.JobExcutor;
using catalog.Models;
using catalog.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<JobBase>> GetAllTasks()
        {
            try
            {
                var now = DateTime.UtcNow.Ticks;
                var jobs = await _context.Jobs
                 .AsNoTracking()
                 .Where(j => j.NextRunTime < now)
                 .Include(j => j.JobSchedule)
                 .Select(j => new Job
                 {
                     Id = j.Id,
                     Type = j.Type,
                     Status = j.Status,
                     NextRunTime = j.NextRunTime,
                     JobSchedule = j.JobSchedule == null
                         ? null
                         : new JobSchedule
                         {
                             Interval = j.JobSchedule.Interval,
                             JobInternalType = j.JobSchedule.JobInternalType
                         }
                 })
                 .ToListAsync();

                var result = new List<JobBase>(jobs.Count);
                foreach (var job in jobs)
                {
                    if (TryAssembleTask(job, out var task))
                    {
                        result.Add(task);
                    }
                }

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error GetAllTasks task: {ex.Message}");
                return new List<JobBase>();

            }

        }

        private bool TryAssembleTask(Job job, out JobBase task)
        {
            try
            {
                task = _jobFactory.GetTaskExecutor(job.Type);
                task.Id = job.Id;
                task.Type = job.Type;
                task.NextRunTime = job.NextRunTime;
                task.Status = job.Status;

                if (job.JobSchedule != null)
                {
                    task.JobSchedule = new JobSchedule
                    {
                        Interval = job.JobSchedule.Interval,
                        JobInternalType = job.JobSchedule.JobInternalType
                    };
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assembling task: {ex.Message}");
                task = null!;
                return false;
            }
        }

        public Task<bool> ReleaseTask(string id)
        {
            throw new NotImplementedException();
        }


        private Job ConvertJobBaseToJob(JobBase job)
        {
            var taskSchedule = job.JobSchedule == null
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
