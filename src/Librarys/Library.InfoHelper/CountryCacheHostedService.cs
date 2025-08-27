using Microsoft.Extensions.Hosting;

namespace Library.InfoHelper;

public class CountryCacheHostedService(ICountryCacheService cacheService) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => cacheService.RefreshAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
