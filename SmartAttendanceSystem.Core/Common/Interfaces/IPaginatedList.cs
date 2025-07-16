namespace SmartAttendanceSystem.Core.Common.Interfaces;

public interface IPaginatedList<T> where T : class
{
    public List<T> Items { get; }
    int PageNumber { get; }
    int TotalPages { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}