using Library.Database.Contexts.Public;
using Library.Database.Models.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Library.InfoHelper;

/// <summary>
/// Service responsible for loading countries into <see cref="CountryCache"/>.
/// </summary>
internal class CountryCacheService(
    IServiceScopeFactory scopeFactory,
    ILogger<CountryCacheService> logger
) : ICountryCacheService
{
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PublicDbContext>();
        List<Country> countries = await dbContext.Countries
            .OrderBy(x => x.CountryName)
            .ToListAsync(cancellationToken);
        CountryCache.SetCountries(countries);
        logger.LogInformation("Reloaded {Count} countries into cache", countries.Count);
    }
}

