using Quartz;

namespace QuartzTask.TaskJob
{
    public class TaskSchedulerJob : IJob
    {
        private readonly ILogger<TaskSample1Job> _logger;
        public TaskSchedulerJob(ILogger<TaskSample1Job> logger)
        {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            int TaskJob = (dataMap.Contains("TaskJob") ? dataMap.GetInt("TaskJob") : 0);
            //Console.WriteLine($"測試sample1執行任務:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            _logger.LogInformation("測試 TaskJob:" + TaskJob + " Scheduler執行任務:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return Task.CompletedTask;
        }
    }
}
