using Microsoft.AspNetCore.Mvc;
using Service.WebAPI.Models.Calc;

namespace Service.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalcController(
        ILogger<CalcController> logger
    ) : ControllerBase
    {
        // GET 範例
        [HttpGet("divide")]
        public IActionResult Divide(int a, int b)
        {
            try
            {
                if (b == 0)
                    throw new DivideByZeroException("除數不能為 0");

                var result = a / b;
                return Ok(new { Success = true, Result = result });
            }
            catch (DivideByZeroException ex)
            {
                logger.LogError(ex, "計算失敗：嘗試除以零");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "發生未預期的錯誤");
                return StatusCode(500, new { Success = false, Message = "伺服器錯誤，請稍後再試" });
            }
        }

        // POST 範例 (使用 Model)
        [HttpPost("multiply")]
        public IActionResult Multiply([FromBody] CalcRequest request)
        {
            try
            {
                var result = request.A * request.B;
                return Ok(new { Success = true, Result = result });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "發生未預期的錯誤");
                return StatusCode(500, new { Success = false, Message = "伺服器錯誤，請稍後再試" });
            }
        }
    }
}