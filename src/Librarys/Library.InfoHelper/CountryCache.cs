using Library.Database.Contexts.Public;
using Library.Database.Models.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.InfoHelper;

public static class CountryCache
{
    private static List<Country> _countries = [];

    public static IReadOnlyList<Country> Countries => _countries;

    public static async Task RefreshAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        PublicDbContext dbContext = scope.ServiceProvider.GetRequiredService<PublicDbContext>();
        _countries = await dbContext.Countries
            .OrderBy(x => x.CountryName)
            .ToListAsync(cancellationToken);
    }
}
