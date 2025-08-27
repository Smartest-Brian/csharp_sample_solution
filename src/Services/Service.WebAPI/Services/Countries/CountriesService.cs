using Library.Core.Results;
using Library.Core.Time.Models;
using Library.Core.Time.Services;
using Library.Database.Models.Public;
using Library.InfoHelper;

using Service.WebAPI.Models.Countries;

namespace Service.WebAPI.Services.Countries;

public class CountriesService(
    ILogger<CountriesService> logger,
    ITimezoneService timezoneService
) : ICountriesService
{
    public Task<Result<List<Country>>> GetCountriesAsync()
    {
        try
        {
            List<Country> data = CountryCache.Countries
                .OrderBy(x => x.CountryName)
                .ToList();
            return Task.FromResult(Result<List<Country>>.Ok(data));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CountriesService.GetCountriesAsync Error");
            throw;
        }
    }

    public Task<Result<Country?>> GetCountryByIdAsync(int id)
    {
        Country? item = CountryCache.Countries
            .FirstOrDefault(x => x.Id == id);

        if (item == null)
        {
            return Result<Country?>.Fail("Country not found");
        }

        return Task.FromResult(Result<Country?>.Ok(item));
    }

    public Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName)
    {
        try
        {
            Country? country = CountryCache.Countries
                .FirstOrDefault(x => x.CountryName.ToLower() == countryName.ToLower());

            if (country == null || string.IsNullOrEmpty(country.Timezone))
            {
                return Task.FromResult(Result<LocalTimeResponse>.Fail("Country not found"));
            }

            TimezoneComputationResult tzResult = timezoneService.ComputeLocalTime(country.Timezone);

            return Task.FromResult(Result<LocalTimeResponse>.Ok(new LocalTimeResponse
            {
                CountryName = country.CountryName,
                Timezone = country.Timezone,
                LocalTime = tzResult.LocalTime,
                UtcOffset = tzResult.UtcOffset
            }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CountriesService.GetLocalTimeAsync Error");
            throw;
        }
    }
}
