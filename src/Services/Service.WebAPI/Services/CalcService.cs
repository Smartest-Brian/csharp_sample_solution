using Library.Core.Common;

namespace Service.WebAPI.Services;

public class CalcService(ILogger<CalcService> logger) : ICalcService
{
    public Result<int> Divide(int a, int b)
    {
        try
        {
            if (b == 0)
            {
                return Result<int>.Fail("除數不能為 0");
            }

            var result = a / b;

            return Result<int>.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CalcService.Divide Error");
            throw;
        }
    }

    public Result<int> Multiply(int a, int b)
    {
        try
        {
            var result = a * b;
            return Result<int>.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CalcService.Multiply Error");
            throw;
        }
    }
}
