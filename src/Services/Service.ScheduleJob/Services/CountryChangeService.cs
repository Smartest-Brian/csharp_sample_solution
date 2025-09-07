using System.Text.Json;
using Library.Database.Models.Public;
using Library.RabbitMQ.Services;
using Microsoft.Extensions.Logging;

namespace Service.ScheduleJob.Services;

public class CountryChangeService(IRabbitMqService rabbitMqService, ILogger<CountryChangeService> logger) : ICountryChangeService
{
    public void Subscribe()
    {
        rabbitMqService.Subscribe("queue.change_country_table", message =>
        {
            CountryInfo? country = JsonSerializer.Deserialize<CountryInfo>(message);
            if (country != null)
            {
                logger.LogInformation("Received country change message: {@Country}", country);
            }
            else
            {
                logger.LogWarning("Failed to deserialize country info from message: {Message}", message);
            }
        });
    }
}
