using Microsoft.AspNetCore.Mvc;

using Service.WebAPI.Models.Calc;
using Service.WebAPI.Services;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalcController(
        ICalcService calcService
    ) : ControllerBase
    {
        [HttpGet("divide")]
        public IActionResult Divide(int a, int b)
        {
            var result = calcService.Divide(a, b);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("multiply")]
        public IActionResult Multiply([FromBody] MultiplyRequest request)
        {
            var result = calcService.Multiply(request.A, request.B);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
