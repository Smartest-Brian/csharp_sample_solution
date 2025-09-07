using Library.RabbitMQ.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Service.ScheduleJob.Services;

public class RabbitMqConsumerService : IHostedService
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private readonly string _queueName;

    public RabbitMqConsumerService(
        IRabbitMqService rabbitMqService,
        IConfiguration configuration,
        ILogger<RabbitMqConsumerService> logger)
    {
        _rabbitMqService = rabbitMqService;
        _logger = logger;
        _queueName = configuration.GetValue<string>("RabbitMq:Queue") ?? "schedule-job-queue";
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _rabbitMqService.Subscribe(_queueName, message =>
        {
            _logger.LogInformation("Receive RabbitMQ message: {Message}", message);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

