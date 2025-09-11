using Microsoft.Extensions.Logging;
using Quartz;

namespace Service.Job.Jobs;

public class CountryUpdatedJob(ILogger<CountryUpdatedJob> logger) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        string? message = context.MergedJobDataMap.GetString("message");
        logger.LogInformation("CountryUpdatedJob -------------------------------------------------------------------");
        logger.LogInformation("RabbitMQ message: {Message}", message);

        return Task.CompletedTask;
    }
}
