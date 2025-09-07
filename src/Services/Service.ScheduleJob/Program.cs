using Library.Core.Logging;
using Library.Core.Middlewares;
using Library.Database.Contexts.Public;
using Library.RabbitMQ.Options;
using Library.RabbitMQ.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Service.ScheduleJob.Jobs;
using Service.ScheduleJob.Services;

namespace Service.ScheduleJob
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ConfigBasic(builder);
            ConfigService(builder);
            ConfigRabbitMq(builder);
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

        private static void ConfigService(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICountryChangeService, CountryChangeService>();
        }

        private static void ConfigRabbitMq(WebApplicationBuilder builder)
        {
            builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));
            builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
        }

        private static void ConfigDatabase(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("PostgreSql");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection String Not Found.");

            builder.Services.AddDbContext<PublicDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString);
                opt.EnableSensitiveDataLogging();
            });
        }

        private static void ConfigQuartz(WebApplicationBuilder builder)
        {
            builder.Services.AddQuartz(q =>
            {
                var jobKey = new JobKey("TimeReportJob");
                q.AddJob<TimeReportJob>(opts => opts.WithIdentity(jobKey));
                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("TimeReportJob-trigger")
                    .WithCronSchedule("0 * * * * ?"));
            });

            builder.Services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });
        }

        private static void ConfigSerilog(WebApplicationBuilder builder) => builder.UseSerilogLogging();

        private static void ConfigApp(WebApplicationBuilder builder)
        {
            WebApplication app = builder.Build();

            IRabbitMqService rabbit = app.Services.GetRequiredService<IRabbitMqService>();
            ICountryChangeService countryService = app.Services.GetRequiredService<ICountryChangeService>();
            rabbit.Subscribe("queue.change_country_table", countryService.HandleMessage);

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
