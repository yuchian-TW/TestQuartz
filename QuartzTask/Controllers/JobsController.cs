using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl.Matchers;
using QuartzTask.TaskJob;

namespace QuartzTask.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IScheduler _scheduler;

        public JobsController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }
        /// <summary>
        /// 查看所有任務工作列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListScheduleJob()
        {

            List<Dictionary<string, string>> jobs = new();

            var allJobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            foreach (JobKey? jobKey in allJobKeys)
            {
                IJobDetail? jobDetail = await _scheduler.GetJobDetail(jobKey);
                if (jobDetail != null)
                {
                    jobs.Add(new Dictionary<string, string>
                    {
                        { "name", jobDetail.Key.Name }, //名稱
                        { "group", jobDetail.Key.Group },//群組
                        { "type", jobDetail.JobType.ToString() },//類型
                        { "description", jobDetail.Description ?? string.Empty },//描述
                    });
                }
            }
            return Ok(jobs);
        }
        /// <summary>
        /// 查看任務觸發工作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ListScheduleTrigger()
        {
            List<Dictionary<string, string>> triggers = new();

            var ListTriggerKeys = await _scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup());
            foreach (var triggerKey in ListTriggerKeys)
            {
                var trigger = await _scheduler.GetTrigger(triggerKey);
                if (trigger.Key != null)
                {
                    TriggerState triggerState = await _scheduler.GetTriggerState(trigger.Key);
                    triggers.Add(new Dictionary<string, string>
                    {
                        { "name", trigger.Key.Name },//名稱
                        { "group", trigger.Key.Group },//群組
                        { "type", trigger.GetType().ToString() },//類型
                        { "status", triggerState.ToString() },//狀態
                        { "description", trigger.Description ?? string.Empty },//描述
                        { "startTimeUtc", trigger.StartTimeUtc.ToString() },//開始
                        { "endTimeUtc", trigger.EndTimeUtc.ToString() ?? string.Empty },//結束
                        { "nextFireTimeUtc", trigger.GetNextFireTimeUtc().ToString() ?? string.Empty },//下次值行時間
                        { "previousFireTimeUtc", trigger.GetPreviousFireTimeUtc().ToString() ?? string.Empty },//前段執行時間
                    });
                }
            }
            return Ok(triggers);
        }
        /// <summary>
        /// 移除任務工作
        /// </summary>
        /// <param name="TaskJob"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteScheduleJob(int TaskJob = 0)
        {
            string jobName = "DynamicJob" + TaskJob;
            string jobGroup = "group1";
            var jobKey = new JobKey(jobName, jobGroup);
            var jobExists = await _scheduler.CheckExists(jobKey);
            if (!jobExists)
            {
                return NotFound($"Job {jobName} does not exist");
            }
            string TriggerName = "DynamicTrigger" + TaskJob;
            var triggerKey = new TriggerKey($"{TriggerName}", jobGroup);
            var triggerExists = await _scheduler.CheckExists(triggerKey);
            if (!triggerExists)
            {
                Console.WriteLine($"Trigger {triggerKey} does not exist");
                return NotFound($"Trigger {triggerKey} does not exist");
            }

            await _scheduler.UnscheduleJob(triggerKey);
            return Ok("移除任務成功");
        }
        /// <summary>
        /// 建立排程任務
        /// </summary>
        /// <param name="TaskJob"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateScheduleJob(int TaskJob = 0)
        {
            try
            {
                // 建立任務
                IJobDetail job = JobBuilder.Create<TaskSchedulerJob>()
                    .UsingJobData("TaskJob", TaskJob)
                    .WithIdentity("DynamicJob" + TaskJob, "group1")
                    .Build();

                // 建立觸發器,立即執行一次,後續每三秒執行一次共執行10次
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("DynamicTrigger" + TaskJob, "group1")
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(3).WithRepeatCount(10))
                    .Build();

                // 調度任務
                await _scheduler.ScheduleJob(job, trigger);
                return Ok("Job scheduled successfully!");
            }
            catch (SchedulerException ex)
            {
                return StatusCode(500, $"Error scheduling job: {ex.Message}");
            }
        }
    }
}
