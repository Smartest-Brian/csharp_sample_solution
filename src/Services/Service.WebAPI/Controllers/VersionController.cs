using Microsoft.AspNetCore.Mvc;

namespace Service.WebAPI.Controllers;

[ApiController]
[Route("version")] // → /version
public class VersionController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("1.0.0");
}
