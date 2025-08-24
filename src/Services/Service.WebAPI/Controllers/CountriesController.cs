using Library.Database.Contexts.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service.WebAPI.Models.Countries;
using Service.WebAPI.Models;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(
        ILogger<CountriesController> logger,
        PublicDbContext publicDbContext
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await publicDbContext.Countries
                    .OrderBy(x => x.CountryName)
                    .ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CountriesController.GetAll Error");
                return Ok();
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await publicDbContext.Countries.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpGet("localTime/{countryName}")]
        public async Task<IActionResult> GetLocalTime(string countryName)
        {
            try
            {
                var country = await publicDbContext.Countries
                    .FirstOrDefaultAsync(x => x.CountryName.ToLower() == countryName.ToLower());

                if (country == null || string.IsNullOrEmpty(country.Timezone))
                    return NotFound();

                var timezone = TimeZoneInfo.FindSystemTimeZoneById(country.Timezone);
                var localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);

                // 考慮夏令時間
                var offset = timezone.GetUtcOffset(localTime);
                string utcOffset = offset.ToString(@"hh\:mm");
                if (!offset.ToString().StartsWith("-"))
                {
                    utcOffset = "+" + utcOffset;
                }

                LocalTimeResponseData res = new LocalTimeResponseData
                {
                    CountryName = country.CountryName,
                    Timezone = country.Timezone,
                    LocalTime = localTime,
                    UtcOffset = utcOffset
                };

                return Ok(new LocalTimeResponse
                {
                    Success = true,
                    Message = null,
                    Data = res
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CountriesController.GetLocalTime Error for country: {CountryName}", countryName);
                return StatusCode(500);
            }
        }
    }
}