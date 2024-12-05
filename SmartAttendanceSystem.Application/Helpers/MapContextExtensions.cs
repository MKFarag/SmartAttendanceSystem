namespace SmartAttendanceSystem.Application.Helpers;

public static class MapContextExtensions
{
    public static void Set(this MapContext context, string key, object value) =>
        context.Parameters[key] = value;

    public static T Get<T>(this MapContext context, string key)
    {
        if (context.Parameters.TryGetValue(key, out var value))
            return (T)value;

        throw new KeyNotFoundException($"Key '{key}' not found in MapContext");
    }
}
