namespace CleanArchitecture.Application.Common.Time;

/// <summary>MVP: fixed UTC+2 for Egypt (no historical DST).</summary>
public static class EgyptTime
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(2);

    public static DateTimeOffset ToDateTimeOffset(DateOnly date, TimeOnly time) =>
        new(date.ToDateTime(time), Offset);
}
