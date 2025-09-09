namespace Library.RabbitMQ.Services;

public interface IRabbitMqService
{
    Task PublishAsync(string exchange, string routingKey, string message);

    /// <summary>
    ///     Subscribe to a queue and bind to the specified routing keys. When a message is received
    ///     the <see cref="MessageReceived"/> event will be triggered with the routing key and message body.
    /// </summary>
    /// <param name="exchange">Exchange name.</param>
    /// <param name="queue">Queue name.</param>
    /// <param name="exchangeType">Exchange type (e.g. direct, topic).</param>
    /// <param name="routingKeys">Routing keys to bind.</param>
    void Subscribe(string exchange, string queue, string exchangeType, params string[] routingKeys);

    /// <summary>
    ///     Event raised when a message is received from RabbitMQ.
    ///     Parameters are routing key and message body.
    /// </summary>
    event Func<string, string, Task>? MessageReceived;
}
