using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Library.ApiClient.Services.Auth;

public static class AuthApiClientExtensions
{
    /// <summary>
    /// Register the Auth API Refit client using configuration for the base address.
    /// </summary>
    public static IServiceCollection AddAuthApiClient(this IServiceCollection services, IConfiguration configuration)
    {
        string? baseUrl = configuration["AuthApi:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new InvalidOperationException("AuthApi:BaseUrl is not configured.");

        services
            .AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        return services;
    }
}
