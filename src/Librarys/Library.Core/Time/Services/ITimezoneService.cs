using Library.Core.Time.Models;

namespace Library.Core.Time.Services;

public interface ITimezoneService
{
    TimezoneComputationResult ComputeLocalTime(string timezoneId, DateTime? utcNow = null);
}