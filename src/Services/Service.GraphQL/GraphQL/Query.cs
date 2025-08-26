using Library.Database.Contexts.Public;
using Library.Database.Models.Public;

using Service.GraphQL.Models;

namespace Service.GraphQL.GraphQL;

public class Query
{
    /// <summary>
    /// 取得 API 版本
    /// </summary>
    public string Version => "1.0.0";

    /// <summary>
    /// 取回目前所有訊息
    /// </summary>
    public IEnumerable<Models.Message> GetMessages()
        => MessageStore.Messages;

    public IQueryable<Country> GetCountries([Service] PublicDbContext db) =>
        db.Countries;
}
