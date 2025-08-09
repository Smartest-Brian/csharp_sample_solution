using HotChocolate.Subscriptions;

namespace GraphQL;

public class Mutation
{
    /// <summary>
    /// 新增訊息並推播給訂閱者
    /// </summary>
    public async Task<Models.Message> AddMessage(
        string text,
        [Service] ITopicEventSender sender)
    {
        var msg = Models.MessageStore.Add(text);
        await sender.SendAsync(nameof(Subscription.OnMessageAdded), msg);
        return msg;
    }
}