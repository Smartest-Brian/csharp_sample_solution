namespace Library.RabbitMQ.Services;

public interface IRabbitMqService
{
    Task PublishAsync(string exchange, string routingKey, string message);
    Task RmqEventDispatcher(string queue, string message);
}
