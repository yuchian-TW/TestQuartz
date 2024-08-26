using Quartz;
using QuartzTask.TaskJob;
/// <summary>
///  用於封裝組態 Quartz 工作 並帶入該工作時間設定
/// </summary>
namespace QuartzTask.Helper
{
    public static class ServiceQuartzConfigExtensions
    {
        public static void AddJobAndTrigger<T>(
       this IServiceCollectionQuartzConfigurator quartz,
       IConfiguration config)
       where T : IJob
        {
            // Use the name of the IJob as the appsettings.json key
            string jobName = typeof(T).Name;

            // Try and load the schedule from configuration
            var configKey = $"Quartz:{jobName}";
            var cronSchedule = config.GetValue<string>(configKey);
            // Some minor validation
            if (string.IsNullOrEmpty(cronSchedule))
            {
                throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

            // register the job as before
            var jobKey = new JobKey(jobName);
            quartz.AddJob<T>(opts => { opts.WithIdentity(jobKey);});
            quartz.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(jobName + "_trigger")
                .WithCronSchedule(cronSchedule)); // use the schedule from configuration
        }
    }
}
