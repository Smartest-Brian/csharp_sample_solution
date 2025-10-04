using Service.GraphQL.Models;

namespace Service.GraphQL.GraphQL;

public class Subscription
{
    /// <summary>
    ///     當有新訊息時即時通知
    /// </summary>
    [Subscribe]
    public Message OnMessageAdded(
        [EventMessage] Message message)
    {
        return message;
    }
}
