using Library.Core.Results;
using Library.Database.Models.Public;

using Microsoft.AspNetCore.Mvc;

using Service.WebAPI.Models.Countries;
using Service.WebAPI.Services.Countries;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(
        ICountriesService countriesService
    ) : ControllerBase
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetList()
        {
            Result<List<CountryInfo>> result = await countriesService.GetCountriesAsync();
            return result.Success
                ? Ok(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id
        )
        {
            Result<CountryInfo?> result = await countriesService.GetCountryByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("localTime/{countryName}")]
        public async Task<IActionResult> GetLocalTime(
            string countryName
        )
        {
            Result<LocalTimeResponse> result = await countriesService.GetLocalTimeAsync(countryName);
            if (result.Success) return Ok(result);
            return result.Message == "Country not found"
                ? NotFound(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
