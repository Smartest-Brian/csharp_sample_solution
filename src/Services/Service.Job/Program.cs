using Library.Core.Logging;
using Library.Core.Middlewares;
using Library.Database.Contexts.Public;
using Library.RabbitMQ.Options;
using Library.RabbitMQ.Services;

using Microsoft.EntityFrameworkCore;

using Quartz;

using Service.Job.Jobs;

namespace Service.Job;

internal static class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        ConfigBasic(builder);
        ConfigDatabase(builder);
        ConfigQuartz(builder);
        ConfigRabbitMq(builder);
        ConfigSerilog(builder);
        ConfigApp(builder);
    }

    private static void ConfigBasic(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddCors();
    }

    private static void ConfigDatabase(WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("PostgreSql");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException($"Connection String Not Found.");

        builder.Services.AddDbContext<PublicDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString);
            opt.EnableSensitiveDataLogging();
        });
    }

    private static void ConfigQuartz(WebApplicationBuilder builder)
    {
        // 加入 Quartz
        builder.Services.AddQuartz(q =>
        {
            // 註冊 Job

            JobKey timeReportJobKey = new("JOB-TimeReport", "STATIC");
            TriggerKey timeReportTriggerKey = new("Trigger-TimeReport", "STATIC");
            q.AddJob<TimeReportJob>(opts => opts.WithIdentity(timeReportJobKey));
            q.AddTrigger(opts => opts
                .ForJob(timeReportJobKey)
                .WithIdentity(timeReportTriggerKey)
                .WithCronSchedule("0 * * * * ?")
            );

            JobKey countryUpdatedJobKey = new("JOB-CountryUpdated", "STATIC");
            q.AddJob<CountryUpdatedJob>(opts => opts
                    .WithIdentity(countryUpdatedJobKey)
                    .StoreDurably() // durable 才能只用 TriggerJob 觸發
                    .RequestRecovery() // 可選：當 Scheduler 異常後恢復
            );
        });

        // 啟用 Quartz Hosted Service
        builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
    }

    private static void ConfigSerilog(WebApplicationBuilder builder)
    {
        builder.UseSerilogLogging();
    }

    private static void ConfigRabbitMq(WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
    }

    private static void ConfigRabbitMqSubscriber(WebApplication app)
    {
        ISchedulerFactory schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        IScheduler scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();

        IRabbitMqService rabbitMqService = app.Services.GetRequiredService<IRabbitMqService>();
        rabbitMqService.Subscribe("exchange.change_country_table", "queue.change_country_table", "key.change_country_table");
        rabbitMqService.MessageReceived += async (routingKey, _) =>
        {
            if (routingKey == "key.change_country_table")
            {
                await scheduler.TriggerJob(new JobKey("JOB-CountryUpdated", "STATIC"));
            }
        };
    }

    private static void ConfigApp(WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        app.UseHttpsRedirection();

        app.UseMiddleware<RequestIdMiddleware>();

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthentication();
        app.UseAuthorization();

        ConfigRabbitMqSubscriber(app);

        app.MapControllers();
        app.Run();
    }
}
