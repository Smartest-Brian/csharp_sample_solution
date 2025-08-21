using Library.Database.Contexts.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController(
        ILogger<CountriesController> _logger,
        PublicDbContext _publicDbContext) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _publicDbContext.Countries
                    .OrderBy(x => x.CountryName)
                    .ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CountriesController.GetAll Error");
                return Ok();
            }
            
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _publicDbContext.Countries.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }
    }
}