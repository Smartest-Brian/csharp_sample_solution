using Library.ApiClient.Attributes;
using Library.ApiClient.Models.Auth;
using Library.Core.Results;
using Library.Database.Models.Public;

using Microsoft.AspNetCore.Mvc;

using Service.WebAPI.Models.Country;
using Service.WebAPI.Services.Country;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(
        ICountryService countryService
    ) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            Result<List<CountryInfo>> result = await countryService.GetCountriesAsync();
            return result.Success
                ? Ok(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id
        )
        {
            Result<CountryInfo?> result = await countryService.GetCountryByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [ValidateToken]
        [HttpGet("localTime/{countryName}")]
        public async Task<IActionResult> GetLocalTime(
            string countryName
        )
        {
            if (HttpContext.Items["TokenInfo"] is not ValidateTokenResponse tokenInfo)
            {
                return Unauthorized();
            }

            if (!tokenInfo.Roles.Contains("admin")) return Forbid();

            Result<LocalTimeResponse> result = await countryService.GetLocalTimeAsync(countryName);
            if (result.Success) return Ok(result);
            return result.Message == "Country not found"
                ? NotFound(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [ValidateToken]
        [HttpPost("add")]
        public async Task<IActionResult> InsertCountry(
            [FromBody] CreateCountryRequest request
        )
        {
            Result<CountryInfo> result = await countryService.AddCountryAsync(request);
            return result.Success
                ? Ok(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
