using Library.Core.Results;
using Library.Core.Time.Models;
using Library.Core.Time.Services;
using Library.Database.Contexts.Public;
using Library.Database.Models.Public;

using Microsoft.EntityFrameworkCore;

using Service.WebAPI.Models.Country;

namespace Service.WebAPI.Services.Country;

public class CountryService(
    PublicDbContext dbContext,
    ILogger<CountryService> logger,
    ITimezoneService timezoneService
) : ICountryService
{
    public async Task<Result<List<CountryInfo>>> GetCountriesAsync()
    {
        try
        {
            List<CountryInfo> data = await dbContext.CountryInfo
                .OrderBy(x => x.CountryName)
                .ToListAsync();

            return Result<List<CountryInfo>>.Ok(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CountriesService.GetCountriesAsync Error");
            throw;
        }
    }

    public async Task<Result<CountryInfo?>> GetCountryByIdAsync(int id)
    {
        CountryInfo? item = await dbContext.CountryInfo
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            return Result<CountryInfo?>.Fail("Country not found");
        }

        return Result<CountryInfo?>.Ok(item);
    }

    public async Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName)
    {
        try
        {
            CountryInfo? country = await dbContext.CountryInfo
                .FirstOrDefaultAsync(x => x.CountryName.ToLower() == countryName.ToLower());

            if (country == null || string.IsNullOrEmpty(country.Timezone))
            {
                return Result<LocalTimeResponse>.Fail("Country not found");
            }

            TimezoneComputationResult tzResult = timezoneService.ComputeLocalTime(country.Timezone);

            return Result<LocalTimeResponse>.Ok(new LocalTimeResponse
            {
                CountryName = country.CountryName,
                Timezone = country.Timezone,
                LocalTime = tzResult.LocalTime,
                UtcOffset = tzResult.UtcOffset
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CountriesService.GetLocalTimeAsync Error");
            throw;
        }
    }
}
