using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzTask.Helper;
using QuartzTask.TaskJob;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
//--���o appsettings.json �]�w�ɸ�T
Microsoft.Extensions.Configuration.ConfigurationManager Configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//--���U���Ƚի׵{��
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
// �`�J IScheduler �իפu�t
builder.Services.AddSingleton(provider =>
{
    var schedulerFactory = provider.GetService<ISchedulerFactory>();
    var scheduler = schedulerFactory?.GetScheduler().GetAwaiter().GetResult();
    scheduler.JobFactory = provider.GetService<IJobFactory>();
    scheduler.Start().GetAwaiter().GetResult();
    return scheduler;
});
//--�إ�Ĳ�o����
builder.Services.AddQuartz(q =>
{
    // Use a Scoped container to create jobs. I'll touch on this later
    //q.UseMicrosoftDependencyInjectionScopedJobFactory();
    //�`�J�u�@����
    q.UseMicrosoftDependencyInjectionJobFactory();
    // ���U�@�~�A�q�]�w���J�p�e
    //q.AddJobAndTrigger<TaskSample1Job>(Configuration);
});
// Add the Quartz.NET hosted service: �����ε{�ǥ��b�����ɡA���|���ݩҦ����b�B�檺�@�~������~�|��������A�ȡC
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
