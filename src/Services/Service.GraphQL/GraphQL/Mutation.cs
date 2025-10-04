using HotChocolate.Subscriptions;

using Service.GraphQL.Models;

namespace Service.GraphQL.GraphQL;

public class Mutation
{
    /// <summary>
    ///     新增訊息並推播給訂閱者
    /// </summary>
    public async Task<Message> AddMessage(
        string text,
        [Service] ITopicEventSender sender)
    {
        Message msg = MessageStore.Add(text);
        await sender.SendAsync(nameof(Subscription.OnMessageAdded), msg);
        return msg;
    }
}
