using System.Text;

using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Service.Job.RabbitMq;

/// <summary>
///     Consumer that handles messages from RabbitMQ and dispatches them via <see cref="RmqHostService" />.
/// </summary>
public class RmqReceiver : DefaultBasicConsumer
{
    private readonly IModel _channel;
    private readonly ILogger _logger;

    public RmqReceiver(IModel channel, ILogger logger) : base(channel)
    {
        _channel = channel;
        _logger = logger;
    }

    public override void HandleBasicDeliver(string consumerTag,
        ulong deliveryTag,
        bool redelivered,
        string exchange,
        string routingKey,
        IBasicProperties properties,
        ReadOnlyMemory<byte> body)
    {
        _logger.LogDebug("==> ExecuteAsync");

        try
        {
            _logger.LogDebug("Consuming Message");
            _logger.LogDebug("Message received from the exchange: {Exchange}", exchange);
            _logger.LogDebug("Routing key: {RoutingKey}", routingKey);
            string bodyString = Encoding.UTF8.GetString(body.ToArray());

            _logger.LogInformation("Message : {Body}", bodyString);

            RmqHostService.AddMessage(routingKey, bodyString);
            _ = RmqHostService.TriggerEvent(routingKey);

            _channel.BasicAck(deliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling message");
        }

        _logger.LogDebug("<== ExecuteAsync");
    }
}

