using catalog.Enumerate;

namespace catalog.JobExcutor
{
    public interface IJobFactory
    {
        JobBase GetTaskExecutor(JobType taskType);
    }
}
