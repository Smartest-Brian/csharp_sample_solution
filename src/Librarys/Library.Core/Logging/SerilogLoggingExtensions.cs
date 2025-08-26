using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Library.Core.Logging;

public static class SerilogLoggingExtensions
{
    /// <summary>
    /// Configure Serilog for the host using app configuration and DI services.
    /// </summary>
    private static IHostBuilder UseSerilogLogging(this IHostBuilder builder)
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

    /// <summary>
    /// Configure Serilog for WebApplicationBuilder, forwarding to Host.
    /// 讓 Program 可直接呼叫 builder.UseSerilogLogging()，更直覺。
    /// </summary>
    public static WebApplicationBuilder UseSerilogLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilogLogging();
        return builder;
    }
}
