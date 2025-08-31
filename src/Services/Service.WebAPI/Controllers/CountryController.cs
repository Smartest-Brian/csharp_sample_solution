using Library.Core.Results;
using Library.Database.Models.Public;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Service.WebAPI.Models.Country;
using Service.WebAPI.Models.Auth;
using Refit;
using Service.WebAPI.Services.Auth;
using Service.WebAPI.Services.Country;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController(
        ICountryService countryService,
        IAuthApi authApi
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Insert(
            [FromBody] CreateCountryRequest request
        )
        {
            string? authorization = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorization)) return Unauthorized();

            ApiResponse<Result<UserInfoResponse>> authResponse = await authApi.GetUserInfoAsync(authorization);

            if (!authResponse.IsSuccessStatusCode || authResponse.Content?.Data == null || !authResponse.Content.Success)
            {
                return Unauthorized();
            }

            if (!authResponse.Content.Data.Roles.Contains("admin")) return Forbid();

            Result<CountryInfo> result = await countryService.AddCountryAsync(request);
            return result.Success
                ? Ok(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
