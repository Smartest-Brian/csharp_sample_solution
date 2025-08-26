using Library.Core.Time.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Library.Core.Time.Extensions;

public static class TimezoneExtensions
{
    public static IServiceCollection AddTimezoneService(this IServiceCollection services)
        => services.AddSingleton<ITimezoneService, TimezoneService>();
}
