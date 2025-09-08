using Quartz;

namespace Service.Job.Jobs;

public class TimeReportJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        DateTime now = DateTime.Now;
        Console.WriteLine($"[{now:yyyy-MM-dd HH:mm:ss}] 現在時間：{now:T}");
        return Task.CompletedTask;
    }
}
