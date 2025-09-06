using Library.RabbitMQ.Services;
using System.Text.Json;
using Library.ApiClient.Attributes;
using Library.Core.Results;
using Library.Database.Models.Public;

using Microsoft.AspNetCore.Mvc;

using Service.WebAPI.Models.Country;
using Service.WebAPI.Services.Country;

namespace Service.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountryController(
    ILogger<CountryController> logger,
    IRabbitMqService rabbitMqService,
    ICountryService countryService
) : ControllerBase
{
    [ValidateToken]
    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        Result<List<CountryInfo>> result = await countryService.GetCountriesAsync();
        return result.Success
            ? Ok(result)
            : StatusCode(StatusCodes.Status500InternalServerError, result);
    }

    [ValidateToken(Roles = ["user"])]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id
    )
    {
        Result<CountryInfo?> result = await countryService.GetCountryByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [ValidateToken(Roles = ["admin", "api"])]
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

    [ValidateToken(Roles = ["admin"])]
    [HttpPost("add")]
    public async Task<IActionResult> InsertCountry(
        [FromBody] CreateCountryRequest request
    )
    {
        logger.LogInformation("Inserting country with request: {@Request}", request);
        Result<CountryInfo> result = await countryService.AddCountryAsync(request);
        if (result.Success && result.Data != null)
        {
            string message = JsonSerializer.Serialize(result.Data);
            await rabbitMqService.PublishAsync("exchange.change_country_table", "key.change_country_table", message);
        }
        return result.Success
            ? Ok(result)
            : StatusCode(StatusCodes.Status500InternalServerError, result);
    }
}
