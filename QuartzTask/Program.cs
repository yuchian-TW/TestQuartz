using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzTask.Helper;
using QuartzTask.TaskJob;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
//--取得 appsettings.json 設定檔資訊
Microsoft.Extensions.Configuration.ConfigurationManager Configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    // Use a Scoped container to create jobs. I'll touch on this later
    //q.UseMicrosoftDependencyInjectionScopedJobFactory();
    //注入工作場域
    q.UseMicrosoftDependencyInjectionJobFactory();
    // 註冊作業，從設定載入計畫
    //q.AddJobAndTrigger<TaskSample1Job>(Configuration);
});
// Add the Quartz.NET hosted service: 當應用程序正在關閉時，它會等待所有正在運行的作業完成後才會完全停止服務。
//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
