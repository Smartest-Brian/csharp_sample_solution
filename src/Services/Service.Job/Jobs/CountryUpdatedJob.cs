using Quartz;

namespace Service.Job.Jobs;

public class CountryUpdatedJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("CountryUpdatedJob -------------------------------------------------------------------");

        return Task.CompletedTask;
    }
}
