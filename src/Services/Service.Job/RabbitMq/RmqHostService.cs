using System.Collections.Concurrent;

using Library.RabbitMQ;
using Library.RabbitMQ.Options;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Service.Job.RabbitMq;

/// <summary>
///     Background service that listens to RabbitMQ queue and dispatches events to <see cref="RmqEventDispatcher" />.
/// </summary>
public class RmqHostService : IHostedService, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RmqHostService> _logger;

    private IConnection? _connection;
    private IModel? _channel;
    private RmqReceiver? _consumer;

    private static readonly ConcurrentDictionary<string, string> Messages = new();

    public RmqHostService(IOptions<RabbitMqOptions> options, ILogger<RmqHostService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        ConnectionFactory factory = new()
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        const string exchangeName = "exchange.change_country_table";
        const string queueName = "queue.change_country_table";
        const string routingKey = "key.change_country_table";

        _channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, durable: true);
        _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queueName, exchangeName, routingKey);

        _consumer = new RmqReceiver(_channel, _logger);
        _channel.BasicConsume(queueName, autoAck: false, consumer: _consumer);

        _logger.LogInformation("RabbitMQ listener started for queue: {Queue}", queueName);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ listener stopping");
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }

    public static void AddMessage(string routingKey, string message) => Messages[routingKey] = message;

    public static bool TryGetMessage(string routingKey, out string? message) => Messages.TryGetValue(routingKey, out message);

    public static Task TriggerEvent(string routingKey) => RmqEventDispatcher.DispatchAsync(routingKey);

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

