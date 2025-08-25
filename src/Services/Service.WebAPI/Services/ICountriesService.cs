namespace Service.WebAPI.Services;

using Library.Core.Common;
using Library.Database.Models.Public;
using Service.WebAPI.Models.Countries;

public interface ICountriesService
{
    Task<Result<List<Country>>> GetAllAsync();
    Task<Result<Country?>> GetByIdAsync(int id);
    Task<Result<LocalTimeResponseData>> GetLocalTimeAsync(string countryName);
}
