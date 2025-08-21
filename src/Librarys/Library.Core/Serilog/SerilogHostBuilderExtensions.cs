using Microsoft.Extensions.Hosting;
using Serilog;

namespace Library.Core.Serilog;

public static class SerilogHostBuilderExtensions
{
    public static IHostBuilder UseLibrarySerilog(this IHostBuilder builder)
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