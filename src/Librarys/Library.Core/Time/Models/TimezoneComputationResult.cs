namespace Library.Core.Time.Models;

public sealed class TimezoneComputationResult
{
    public DateTime LocalTime { get; init; }
    public string UtcOffset { get; init; } = "";
}
