namespace CongestionTaxCalculator;

public static class Extensions
{
    public static bool IsWeekend(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public static bool IsBetweenInclusive(
        this TimeOnly time,
        TimeOnly start,
        TimeOnly end)
    {
        return time.IsBetween(start, end)
            || time == start
            || time == end;
    }
}
