using Library.Core.Logging;
using Library.Core.Middlewares;
using Library.Database.Contexts.Public;
using Library.RabbitMQ;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

using Quartz;

using Service.ScheduleJob.Jobs;

namespace Service.ScheduleJob
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ConfigBasic(builder);
            ConfigDatabase(builder);
            ConfigQuartz(builder);
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

        private static void ConfigSerilog(WebApplicationBuilder builder) => builder.UseSerilogLogging();

        private static void ConfigRmqEventDispatcher(WebApplication app)
        {
            // 取得 Quartz Scheduler 並註冊 RabbitMQ 事件
            ISchedulerFactory schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
            IScheduler scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();

            RmqEventDispatcher.Register("key.change_country_table", async () => { await scheduler.TriggerJob(new JobKey("JOB-CountryUpdated", "STATIC")); });
        }

        private static void ConfigApp(WebApplicationBuilder builder)
        {
            WebApplication app = builder.Build();

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestIdMiddleware>();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            ConfigRmqEventDispatcher(app);

            app.MapControllers();
            app.Run();
        }
    }
}
