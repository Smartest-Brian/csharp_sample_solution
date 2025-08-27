using Library.Database.Contexts.Public;
using Library.Database.Models.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.InfoHelper;

public class CountryCacheHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<CountryCacheHostedService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PublicDbContext>();
        List<Country> countries = await dbContext.Countries
            .OrderBy(x => x.CountryName)
            .ToListAsync(cancellationToken);
        CountryCache.SetCountries(countries);
        logger.LogInformation("Loaded {Count} countries into cache", countries.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
