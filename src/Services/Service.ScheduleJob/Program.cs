using Quartz;
using Service.ScheduleJob.Jobs;

var builder = Host.CreateApplicationBuilder(args);

// 加入 Quartz
builder.Services.AddQuartz(q =>
{
    // 註冊 Job
    var jobKey = new JobKey("TimeReportJob");
    q.AddJob<TimeReportJob>(opts => opts.WithIdentity(jobKey));

    // 建立觸發器：每分鐘執行一次
    q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("TimeReportJob-trigger")
            .WithCronSchedule("0 * * * * ?") // Quartz Cron：每分鐘 0 秒觸發
    );
});

// 啟用 Quartz Hosted Service
builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });

var host = builder.Build();
host.Run();