using System.Threading;
using System.Threading.Tasks;

namespace Library.InfoHelper;

public interface ICountryCacheService
{
    /// <summary>
    /// Reloads the country cache from the database.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken = default);
}

