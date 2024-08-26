<h1 align='left'>
  dotnet core 建立任務排程器範本
</h1>

### 安裝所需套件
1.dotnet add package Quartz
2.dotnet add package Quartz.Extensions.Hosting

### 程式運行建立觸發任務：建立屬於特定作業排程執行參考

1.Program.cs

```C#

builder.Services.AddQuartz(q =>
{
    //....
    // 註冊作業，從設定載入計畫
    q.AddJobAndTrigger<TaskSample1Job>(Configuration);
});

```
2. Helper/ServiceQuartzConfigExtensions.cs 建立用於封裝組態 Quartz 工作 並帶入該工作時間設定

3. TaskJob/TaskSample1Job.cs 建立排程器需執行程式內容，在執行該 Job 實例時不允許同時執行其他相同的 Job

### 建立動態任務排程器：能更靈活的根據需要動態建立任務執行排程，不局限於特定作業上。

1.Program.cs

```C#

//--註冊任務調度程式
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
// 注入 IScheduler 調度工廠
builder.Services.AddSingleton(provider =>
{
    var schedulerFactory = provider.GetService<ISchedulerFactory>();
    var scheduler = schedulerFactory?.GetScheduler().GetAwaiter().GetResult();
    scheduler.JobFactory = provider.GetService<IJobFactory>();
    scheduler.Start().GetAwaiter().GetResult();
    return scheduler;
});

//--建立觸發任務
builder.Services.AddQuartz(q =>
{
    //注入工作場域
    q.UseMicrosoftDependencyInjectionJobFactory();
});

```

2. Controllers/JobsController.cs 建立API：用於透過API 建立動態排程任務執行
+ ListScheduleJob 查看所有任務工作列表
+ ListScheduleTrigger 查看所有任務觸發工作資訊，包含任務名稱、狀態、開始時間、下次執行時間
+ DeleteScheduleJob 移除任務工作
+ DeleteScheduleJob 建立任務工作

