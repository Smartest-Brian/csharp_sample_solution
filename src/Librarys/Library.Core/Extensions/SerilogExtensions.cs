using Microsoft.Extensions.Hosting;

using Serilog;

namespace Library.Core.Extensions;

public static class SerilogExtensions
{
    public static IHostBuilder UseSerilogExtensions(this IHostBuilder builder)
    {
        builder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        return builder;
    }
}
