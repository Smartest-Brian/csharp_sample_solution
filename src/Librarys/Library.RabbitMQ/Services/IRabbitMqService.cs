namespace Library.RabbitMQ.Services;

public interface IRabbitMqService
{
    Task PublishAsync(string exchange, string routingKey, string message);
    void Subscribe(string exchange, string queue, string routingKey, Action<string> onMessage);
}
