using Library.RabbitMQ.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.RabbitMQ;

public class RabbitListenerHostedService(IRabbitMqService rabbitMqService, ILogger<RabbitListenerHostedService> logger) : BackgroundService
{
    private readonly IRabbitMqService _rabbitMqService = rabbitMqService;
    private readonly ILogger<RabbitListenerHostedService> _logger = logger;
    private const string QueueName = "queue.change_country_table";
    private const string EventKey = "key.change_country_table";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMqService.Subscribe(QueueName, async message =>
        {
            _logger.LogInformation("Received message: {Message}", message);
            await RmqEventDispatcher.DispatchAsync(EventKey);
        });

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
