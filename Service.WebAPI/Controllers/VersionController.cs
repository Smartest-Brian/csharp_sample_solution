using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("version")]          // → /version
public class VersionController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("1.0.0");
}