using Models;
using HotChocolate;
using Microsoft.Extensions.Logging;

namespace GraphQL;

public class Query
{
    /// <summary>
    /// 取得 API 版本
    /// </summary>
    public string Version => "1.0.0";

    /// <summary>
    /// 取回目前所有訊息
    /// </summary>
    public IEnumerable<Models.Message> GetMessages([Service] ILogger<Query> logger)
    {
        logger.LogInformation("Fetching messages");
        return MessageStore.Messages;
    }
}
