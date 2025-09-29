using catalog.JobExcutor;
using catalog.Models;

namespace catalog.Repository.Interfaces
{
    public interface IJobRepository
    {
        Task<List<Job>> GetAllTasks();
        Task<bool> ReleaseTask(string id);
        Task<bool> CreateTask(JobBase job);
        Task InitTask();
    }
}
