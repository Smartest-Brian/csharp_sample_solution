namespace Service.WebAPI.Models.Country
{
    public class LocalTimeResponse
    {
        public string CountryName { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public DateTime? LocalTime { get; set; } = null;
        public string UtcOffset { get; set; } = string.Empty;
    }
}
