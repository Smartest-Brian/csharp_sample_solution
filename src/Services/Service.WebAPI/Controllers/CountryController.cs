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

        [HttpGet("localTime/{countryName}")]
        public async Task<IActionResult> GetLocalTime(
            string countryName
        )
        {
            Result<LocalTimeResponse> result = await countryService.GetLocalTimeAsync(countryName);
            if (result.Success) return Ok(result);
            return result.Message == "Country not found"
                ? NotFound(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPost("add")]
        [ValidateToken]
        public async Task<IActionResult> InsertCountry(
            [FromBody] CreateCountryRequest request
        )
        {
            if (HttpContext.Items["UserInfo"] is not UserInfoResponse user)
            {
                return Unauthorized();
            }

            if (!user.Roles.Contains("admin")) return Forbid();

            Result<CountryInfo> result = await countryService.AddCountryAsync(request);
            return result.Success
                ? Ok(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
