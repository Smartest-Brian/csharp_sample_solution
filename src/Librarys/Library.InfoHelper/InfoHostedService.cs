using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Library.InfoHelper;

public class InfoHostedService(IServiceProvider serviceProvider, ILogger<InfoHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await CountryCache.RefreshAsync(serviceProvider, cancellationToken);
        logger.LogInformation("Country cache initialized with {Count} items", CountryCache.Countries.Count);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
