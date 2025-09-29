using Microsoft.Extensions.Logging;
using ScheduleJob.Enumn;
using ScheduleJob.JobExcutor;
using ScheduleJob.Model;
using ScheduleJob.Repository;


namespace ScheduleJob
{
    public class WorkerRole
    {
        private readonly ILogger<WorkerRole> _logger;
        private List<TaskType> excludeTasks = new List<TaskType>() { };
        private readonly ITaskScheduleRepository _jobScheduleRepository;

        public WorkerRole(ILogger<WorkerRole> logger, ScheduleDbContext context, ITaskScheduleRepository jobScheduleRepository)
        {
            _logger = logger;
            _jobScheduleRepository = jobScheduleRepository;
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Worker running...");
            // TODO: logic của bạn, ví dụ:
            // var count = await _context.ScheduledTasks.CountAsync();
            // _logger.LogInformation("Tasks: {Count}", count);
            while (true)
            {
                var tasks = await _jobScheduleRepository.GetAllTasks();
                _logger.LogInformation($"Total Tasks: {tasks.Count}");
                foreach (var task in tasks)
                {
                    _logger.LogInformation($"Process task: {task.Id}, task type: {task.Type}");
                    try
                    {
                        ThreadPool.QueueUserWorkItem(Process, task);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing task: {task.Id}, task type: {task.Type}, message: {ex.Message}");
                    }

                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private void Process(object o)
        {
            TaskBase task = (TaskBase)o;
            try
            {
                _logger.LogInformation($"Start processing task: {task.Id}, task type: {task.Type}");
                var taskExecutor = JobExcutorFactory.GetJobExcutor(task.Type);
                taskExecutor.ExecutorAsync(task).Wait();
                _logger.LogInformation($"Finished processing task: {task.Id}, task type: {task.Type}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing task: {task.Id}, task type: {task.Type}, message: {ex.Message}");
            }
        }

        public async Task InitTask()
        {
            try
            {
                var taskTypes = Enum.GetValues(typeof(TaskType));
                foreach (var taskType in taskTypes)
                {
                    var type = (TaskType)taskType;
                    if (!excludeTasks.Contains(type))
                    {
                        var job = JobFactory.GetDefaultJob(type);
                        await _jobScheduleRepository.CreateTask(job);
                    }
                    else
                    {
                        _logger.LogInformation($"skip to run job: {type}");
                    }
                }
            }
            catch
            {
            }
        }


    }
}
