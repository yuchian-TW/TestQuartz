using Quartz;
/// sample1: 測試執行任務排成
namespace QuartzTask.TaskJob
{
    //用於標記 Job 類型，指示調度器在執行該 Job 實例時不允許同時執行其他相同的 Job
    [DisallowConcurrentExecution]
    public class TaskSample1Job : IJob
    {
        private readonly ILogger<TaskSample1Job> _logger;
        public TaskSample1Job(ILogger<TaskSample1Job> logger) {
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine($"測試sample1執行任務:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //_logger.LogInformation("測試sample1執行任務:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return Task.CompletedTask;
        }
    }
}
