using Microsoft.Extensions.Logging;
using Quartz;

namespace HourlyQuartzJob.Jobs;

public class ClockJob : IJob
{
    private readonly ILogger<ClockJob> _logger;

    public ClockJob(ILogger<ClockJob> logger) => _logger = logger;

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("ClockJob executed at {Time}", DateTimeOffset.Now);
        // TODO: put your real work here
        return Task.CompletedTask;
    }
}