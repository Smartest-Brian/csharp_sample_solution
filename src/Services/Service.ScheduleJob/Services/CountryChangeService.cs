using System.Text.Json;
using Library.Database.Contexts.Public;
using Library.Database.Models.Public;
using Microsoft.EntityFrameworkCore;

namespace Service.ScheduleJob.Services;

public class CountryChangeService(PublicDbContext dbContext, ILogger<CountryChangeService> logger) : ICountryChangeService
{
    public void HandleMessage(string message)
    {
        try
        {
            CountryInfo? country = JsonSerializer.Deserialize<CountryInfo>(message);
            if (country == null)
            {
                logger.LogWarning("Received invalid country message: {Message}", message);
                return;
            }

            CountryInfo? existing = dbContext.CountryInfo.AsNoTracking().FirstOrDefault(x => x.Id == country.Id);
            if (existing == null)
            {
                dbContext.CountryInfo.Add(country);
            }
            else
            {
                dbContext.CountryInfo.Update(country);
            }
            dbContext.SaveChanges();
            logger.LogInformation("Processed country change for {CountryName}", country.CountryName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process country change message");
        }
    }
}
