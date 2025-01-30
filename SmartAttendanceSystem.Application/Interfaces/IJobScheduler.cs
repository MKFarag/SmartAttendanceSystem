namespace SmartAttendanceSystem.Application.Interfaces;

public interface IJobScheduler
{
    string Enqueue(Expression<Action> methodCall);
    string Enqueue(Expression<Func<Task>> methodCall);

    string Schedule(Expression<Action> methodCall, TimeSpan delay);
    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);
}
