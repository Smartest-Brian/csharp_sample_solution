using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.RabbitMQ.Services;

public class RabbitMqConsumerService(
    IRabbitMqService rabbitMqService,
    ILogger<RabbitMqConsumerService> logger,
    string queueName) : IHostedService
{
    private readonly IRabbitMqService _rabbitMqService = rabbitMqService;
    private readonly ILogger<RabbitMqConsumerService> _logger = logger;
    private readonly string _queueName = queueName;

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

