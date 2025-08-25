using Library.Core.Common;
using Library.Database.Contexts.Public;
using Library.Database.Models.Public;
using Microsoft.EntityFrameworkCore;
using Service.WebAPI.Models.Countries;

namespace Service.WebAPI.Services;

public class CountriesService(
    PublicDbContext dbContext,
    ILogger<CountriesService> logger
) : ICountriesService
{
    public async Task<Result<List<Country>>> GetAllAsync()
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
            logger.LogError(ex, "CountriesService.GetAllAsync Error");
            return Result<List<Country>>.Fail("Server error");
        }
    }

    public async Task<Result<Country?>> GetByIdAsync(int id)
    {
        var item = await dbContext.Countries.FindAsync(id);
        if (item == null)
        {
            return Result<Country?>.Fail("Country not found");
        }

        return Result<Country?>.Ok(item);
    }

    public async Task<Result<LocalTimeResponseData>> GetLocalTimeAsync(string countryName)
    {
        try
        {
            var country = await dbContext.Countries
                .FirstOrDefaultAsync(x => x.CountryName.ToLower() == countryName.ToLower());

            if (country == null || string.IsNullOrEmpty(country.Timezone))
            {
                return Result<LocalTimeResponseData>.Fail("Country not found");
            }

            var timezone = TimeZoneInfo.FindSystemTimeZoneById(country.Timezone);
            var localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);

            // 考慮夏令時間
            var offset = timezone.GetUtcOffset(localTime);
            var utcOffset = offset.ToString(@"hh\:mm");
            if (!offset.ToString().StartsWith("-"))
            {
                utcOffset = "+" + utcOffset;
            }

            return Result<LocalTimeResponseData>.Ok(new LocalTimeResponseData
            {
                CountryName = country.CountryName,
                Timezone = country.Timezone,
                LocalTime = localTime,
                UtcOffset = utcOffset
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CountriesService.GetLocalTimeAsync Error for country: {CountryName}", countryName);
            return Result<LocalTimeResponseData>.Fail("Server error");
        }
    }
}