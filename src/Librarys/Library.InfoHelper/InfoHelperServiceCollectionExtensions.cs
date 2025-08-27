using Microsoft.Extensions.DependencyInjection;

namespace Library.InfoHelper;

public static class InfoHelperServiceCollectionExtensions
{
    public static IServiceCollection AddInfoHelper(this IServiceCollection services)
    {
        services.AddHostedService<CountryCacheHostedService>();
        return services;
    }
}
