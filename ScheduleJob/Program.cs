using HourlyQuartzJob.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

// 1️⃣ Register Quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Job identity
    var jobKey = new JobKey(nameof(ClockJob));

    // Register the job itself
    q.AddJob<ClockJob>(opts => opts.WithIdentity(jobKey));

    // Run every hour on the hour
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity($"{nameof(ClockJob)}-trigger")
        .WithSimpleSchedule(x => x
            .WithInterval(TimeSpan.FromHours(1))
            .RepeatForever()));
});

// 2️⃣ Make Quartz run as a hosted service
builder.Services.AddQuartzHostedService(opts =>
{
    opts.WaitForJobsToComplete = true;   // graceful shutdown
});

var host = builder.Build();
await host.RunAsync();