using Library.Core.Common;

namespace Service.WebAPI.Services.Calc;

public interface ICalcService
{
    Result<int> Divide(int a, int b);
    Result<int> Multiply(int a, int b);
}
