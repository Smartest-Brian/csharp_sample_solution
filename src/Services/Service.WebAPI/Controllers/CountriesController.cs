using Library.Database.Contexts.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ILogger<CountriesController> _logger;
        private readonly PublicDbContext _publicDbContext;

        public CountriesController(
            ILogger<CountriesController> logger,
            PublicDbContext publicDbContext
        )
        {
            _logger = logger;
            _publicDbContext = publicDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _publicDbContext.Countries
                .OrderBy(x => x.CountryName)
                .ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _publicDbContext.Countries.FindAsync(id);
            return item is null ? NotFound() : Ok(item);
        }
    }
}