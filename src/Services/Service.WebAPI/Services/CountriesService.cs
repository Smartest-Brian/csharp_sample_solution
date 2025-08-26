using Library.Core.Common;
using Library.Core.Time.Services;
using Library.Database.Contexts.Public;
using Library.Database.Models.Public;

using Microsoft.EntityFrameworkCore;

using Service.WebAPI.Models.Countries;

namespace Service.WebAPI.Services;

public class CountriesService(
    PublicDbContext dbContext,
    ILogger<CountriesService> logger,
    ITimezoneService timezoneService
) : ICountriesService
{
    public async Task<Result<List<Country>>> GetCountriesAsync()
    {
        try
        {
            var data = await dbContext.Countries
                .OrderBy(x => x.CountryName)
                .ToListAsync();

            return Result<List<Country>>.Ok(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CountriesService.GetCountriesAsync Error");
            return Result<List<Country>>.Fail("Server error");
        }
    }

    public async Task<Result<Country?>> GetCountryByIdAsync(int id)
    {
        var item = await dbContext.Countries
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return Result<Country?>.Fail("Country not found");
        }

        return Result<Country?>.Ok(item);
    }

    public async Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName)
    {
        try
        {
            var country = await dbContext.Countries
                .FirstOrDefaultAsync(x => x.CountryName.ToLower() == countryName.ToLower());

            if (country == null || string.IsNullOrEmpty(country.Timezone))
            {
                return Result<LocalTimeResponse>.Fail("Country not found");
            }

            var tzResult = timezoneService.ComputeLocalTime(country.Timezone);

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
            logger.LogError(ex, "CountriesService.GetLocalTimeAsync Error for country: {CountryName}", countryName);
            return Result<LocalTimeResponse>.Fail("Server error");
        }
    }
}
