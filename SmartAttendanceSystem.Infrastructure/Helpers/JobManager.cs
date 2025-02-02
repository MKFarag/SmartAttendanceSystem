using Hangfire;

namespace SmartAttendanceSystem.Infrastructure.Helpers;

public class JobManager : IJobManager
{
    public string Enqueue(Expression<Action> methodCall)
        => BackgroundJob.Enqueue(methodCall);

    public string Enqueue(Expression<Func<Task>> methodCall)
    => BackgroundJob.Enqueue(methodCall);

    ////////////////////////////////////////////////////////////////////////////////

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
        => BackgroundJob.Schedule(methodCall, delay);
}
