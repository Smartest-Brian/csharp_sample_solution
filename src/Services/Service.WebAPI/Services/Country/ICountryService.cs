using Library.Core.Results;
using Library.Database.Models.Public;

using Service.WebAPI.Models.Country;

namespace Service.WebAPI.Services.Country;

public interface ICountryService
{
    Task<Result<List<CountryInfo>>> GetCountriesAsync();
    Task<Result<CountryInfo?>> GetCountryByIdAsync(int id);
    Task<Result<LocalTimeResponse>> GetLocalTimeAsync(string countryName);
}
