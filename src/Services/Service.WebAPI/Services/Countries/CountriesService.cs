using Library.Core.Results;
using Library.Core.Time.Models;
using Library.Core.Time.Services;
using Library.Database.Models.Public;
using Library.InfoHelper;

using Service.WebAPI.Models.Countries;

namespace Service.WebAPI.Services.Countries;

public class CountriesService(
    ITimezoneService timezoneService
) : ICountriesService
{
    public Task<Result<List<Country>>> GetCountriesAsync()
    {
        List<Country> data = CountryCache.Countries
            .OrderBy(x => x.CountryName)
            .ToList();

        return Task.FromResult(Result<List<Country>>.Ok(data));
    }

    public Task<Result<Country?>> GetCountryByIdAsync(int id)
    {
        Country? item = CountryCache.Countries.FirstOrDefault(x => x.Id == id);
        return Task.FromResult(item != null
            ? Result<Country?>.Ok(item)
            : Result<Country?>.Fail("Country not found"));
    }

    public Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName)
    {
        Country? country = CountryCache.Countries
            .FirstOrDefault(x => string.Equals(x.CountryName, countryName, StringComparison.OrdinalIgnoreCase));

        if (country == null || string.IsNullOrEmpty(country.Timezone))
        {
            return Task.FromResult(Result<LocalTimeResponse>.Fail("Country not found"));
        }

        TimezoneComputationResult tzResult = timezoneService.ComputeLocalTime(country.Timezone);

        LocalTimeResponse response = new()
        {
            CountryName = country.CountryName,
            Timezone = country.Timezone,
            LocalTime = tzResult.LocalTime,
            UtcOffset = tzResult.UtcOffset
        };

        return Task.FromResult(Result<LocalTimeResponse>.Ok(response));
    }
}
