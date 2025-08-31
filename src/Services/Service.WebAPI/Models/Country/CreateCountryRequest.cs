namespace Service.WebAPI.Models.Country;

public class CreateCountryRequest
{
    public string CountryName { get; set; } = string.Empty;
    public string CountryCode2 { get; set; } = string.Empty;
    public string CountryCode3 { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
}
