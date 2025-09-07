using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.RabbitMQ.Services;

public class RabbitMqConsumerService(
    IRabbitMqService rabbitMqService,
    ILogger<RabbitMqConsumerService> logger,
    string queueName) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        rabbitMqService.Subscribe(queueName, message => { logger.LogInformation("Receive RabbitMQ message: {Message}", message); });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
