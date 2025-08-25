using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Service.WebAPI.Models.Countries;
using Service.WebAPI.Services;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(
        ICountriesService countriesService
    ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await countriesService.GetAllAsync();
            return result.Success
                ? Ok(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await countriesService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("localTime/{countryName}")]
        public async Task<IActionResult> GetLocalTime(string countryName)
        {
            var result = await countriesService.GetLocalTimeAsync(countryName);
            if (result.Success) return Ok(result);
            return result.Error == "Country not found"
                ? NotFound(result)
                : StatusCode(StatusCodes.Status500InternalServerError, result);
        }
    }
}
