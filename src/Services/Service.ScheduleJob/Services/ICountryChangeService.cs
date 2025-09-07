namespace Service.ScheduleJob.Services;

public interface ICountryChangeService
{
    void HandleMessage(string message);
}
