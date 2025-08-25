using Library.Core.Time.Models;

namespace Library.Core.Time.Services;

public class TimezoneService : ITimezoneService
{
    public TimezoneComputationResult ComputeLocalTime(string timezoneId, DateTime? utcNow = null)
    {
        var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

        var utc = utcNow?.ToUniversalTime() ?? DateTime.UtcNow;
        var localTime = TimeZoneInfo.ConvertTime(utc, timezone);

        var offset = timezone.GetUtcOffset(localTime);
        var sign = offset < TimeSpan.Zero ? "-" : "+";
        var abs = offset.Duration();
        var utcOffset = $"{sign}{abs:hh\\:mm}";

        return new TimezoneComputationResult
        {
            LocalTime = localTime,
            UtcOffset = utcOffset
        };
    }
}