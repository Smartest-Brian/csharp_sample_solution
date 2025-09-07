namespace Library.RabbitMQ.Services;

public interface IRabbitMqService
{
    Task PublishAsync(string exchange, string routingKey, string message);
    void Subscribe(string queue, Func<string, Task> onMessage);
}
