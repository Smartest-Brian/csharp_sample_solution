namespace Service.WebAPI.Services;

using Library.Core.Common;
using Library.Database.Models.Public;

using Service.WebAPI.Models.Countries;

public interface ICountriesService
{
    Task<Result<List<Country>>> GetCountriesAsync();
    Task<Result<Country?>> GetCountryByIdAsync(int id);
    Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName);
}
