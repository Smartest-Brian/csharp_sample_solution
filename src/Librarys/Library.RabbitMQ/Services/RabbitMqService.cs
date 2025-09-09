using System.Text;

using Library.RabbitMQ.Options;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.RabbitMQ.Services;

public class RabbitMqService(IOptions<RabbitMqOptions> options) : IRabbitMqService, IDisposable
{
    private readonly RabbitMqOptions _options = options.Value;
    private IConnection? _connection;
    private IModel? _channel;
    public event Func<string, string, Task>? MessageReceived;

    private IModel GetOrCreateChannel()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            ConnectionFactory factory = new()
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
        }

        _channel ??= _connection.CreateModel();
        return _channel;
    }

    public Task PublishAsync(string exchange, string routingKey, string message)
    {
        IModel channel = GetOrCreateChannel();
        channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);

        IBasicProperties? properties = channel.CreateBasicProperties();
        properties.Expiration = (10 * 60 * 1000).ToString(); // 10 minutes

        byte[] body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange, routingKey, basicProperties: properties, body: body);
        return Task.CompletedTask;
    }

    public void Subscribe(string exchange, string queue, params string[] routingKeys)
    {
        IModel channel = GetOrCreateChannel();

        channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        foreach (string routingKey in routingKeys)
        {
            channel.QueueBind(queue, exchange, routingKey);
        }

        AsyncEventingBasicConsumer consumer = new(channel);
        consumer.Received += async (_, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            if (MessageReceived != null)
            {
                await MessageReceived.Invoke(ea.RoutingKey, message);
            }
        };

        channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
