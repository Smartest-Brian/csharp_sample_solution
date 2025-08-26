using Library.Core.Time.Models;

namespace Library.Core.Time.Services;

public class TimezoneService : ITimezoneService
{
    public TimezoneComputationResult ComputeLocalTime(string timezoneId, DateTime? utcNow = null)
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

        DateTime utc = utcNow?.ToUniversalTime() ?? DateTime.UtcNow;
        DateTime localTime = TimeZoneInfo.ConvertTime(utc, timezone);

        TimeSpan offset = timezone.GetUtcOffset(localTime);
        var sign = offset < TimeSpan.Zero ? "-" : "+";
        TimeSpan abs = offset.Duration();
        var utcOffset = $"{sign}{abs:hh\\:mm}";

        return new TimezoneComputationResult
        {
            LocalTime = localTime,
            UtcOffset = utcOffset
        };
    }
}
