using Library.Core.Extensions;
using Library.Core.Middlewares;
using Library.Database.Contexts.Public;

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
            var builder = WebApplication.CreateBuilder(args);

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
            var connectionString = builder.Configuration.GetConnectionString("PostgreSql");
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
        }

        private static void ConfigSerilog(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilogExtensions();
        }


        private static void ConfigApp(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseMiddleware<RequestIdMiddleware>();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
