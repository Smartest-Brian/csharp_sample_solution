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

    private IModel GetOrCreateChannel()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            ConnectionFactory factory = new()
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password
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
        byte[] body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange, routingKey, basicProperties: null, body: body);
        return Task.CompletedTask;
    }

    public void Subscribe(string queue, Action<string> onMessage)
    {
        IModel channel = GetOrCreateChannel();
        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
        EventingBasicConsumer consumer = new(channel);
        consumer.Received += (_, ea) =>
        {
            string body = Encoding.UTF8.GetString(ea.Body.ToArray());
            onMessage(body);
        };
        channel.BasicConsume(queue, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
