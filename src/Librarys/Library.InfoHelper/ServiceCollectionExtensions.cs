using Microsoft.Extensions.DependencyInjection;

namespace Library.InfoHelper;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfoHelper(this IServiceCollection services)
    {
        services.AddHostedService<InfoHostedService>();
        return services;
    }
}
