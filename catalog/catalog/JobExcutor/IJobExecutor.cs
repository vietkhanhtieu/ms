namespace catalog.JobExcutor
{
    public interface IJobExecutor
    {
        System.Threading.Tasks.Task ExecutorAsync(JobBase task);
    }
}
