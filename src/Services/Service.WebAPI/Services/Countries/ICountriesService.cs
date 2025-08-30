using Library.Core.Results;
using Library.Database.Models.Public;

using Service.WebAPI.Models.Countries;

namespace Service.WebAPI.Services.Countries;

public interface ICountriesService
{
    Task<Result<List<CountryInfo>>> GetCountriesAsync();
    Task<Result<CountryInfo?>> GetCountryByIdAsync(int id);
    Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName);
}
