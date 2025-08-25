namespace Service.WebAPI.Services;

using Library.Core.Common;

public interface ICalcService
{
    Result<int> Divide(int a, int b);
    Result<int> Multiply(int a, int b);
}
