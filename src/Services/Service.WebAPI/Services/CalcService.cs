namespace Service.WebAPI.Services;

using Library.Core.Common;
using Microsoft.Extensions.Logging;

public class CalcService(ILogger<CalcService> logger) : ICalcService
{
    public Result<int> Divide(int a, int b)
    {
        try
        {
            if (b == 0)
                return Result<int>.Fail("除數不能為 0");

            var result = a / b;
            return Result<int>.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "計算失敗：嘗試除以零");
            return Result<int>.Fail("伺服器錯誤，請稍後再試");
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
            logger.LogError(ex, "發生未預期的錯誤");
            return Result<int>.Fail("伺服器錯誤，請稍後再試");
        }
    }
}
