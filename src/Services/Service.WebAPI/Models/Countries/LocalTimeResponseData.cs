namespace Service.WebAPI.Models.Countries
{
    public class LocalTimeResponseData
    {
        public string CountryName { get; set; } = string.Empty;

        public string Timezone { get; set; } = string.Empty;

        public DateTime? LocalTime { get; set; } = null;

        public string UtcOffset { get; set; } = string.Empty;
    }
}
