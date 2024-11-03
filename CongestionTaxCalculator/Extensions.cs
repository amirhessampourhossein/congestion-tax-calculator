using CongestionTaxCalculator.Entities;
using CongestionTaxCalculator.Entities.Common;

namespace CongestionTaxCalculator;

public static class Extensions
{
    public static bool IsWeekend(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public static bool IsBetweenInclusive(this TimeOnly time, TimeOnly start, TimeOnly end)
    {
        return time.IsBetween(start, end)
            || time == start
            || time == end;
    }

    public static bool IsExemptVehicle(this Vehicle vehicle)
    {
        return vehicle is
            EmergencyVehicle or
            Bus or
            DiplomatVehicle or
            Motorcycle or
            MilitaryVehicle or
            ForeignVehicle;
    }
}
