using HotChocolate;
using HotChocolate.Subscriptions;

namespace GraphQL;

public class Subscription
{
    /// <summary>
    /// 當有新訊息時即時通知
    /// </summary>
    [Subscribe]
    public Models.Message OnMessageAdded(
        [EventMessage] Models.Message message) => message;
}