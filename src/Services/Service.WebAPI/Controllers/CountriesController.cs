using Library.Database.Contexts.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}