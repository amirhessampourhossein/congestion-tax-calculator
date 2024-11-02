using CongestionTaxCalculator.Entities;
using CongestionTaxCalculator.Entities.Common;

namespace CongestionTaxCalculator;

public static class Extensions
{
    public static bool IsWeekend(this DayOfWeek dayOfWeek)
        => dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;

    public static bool IsExemptVehicle(this Vehicle vehicle)
        => vehicle is 
        EmergencyVehicle or 
        Bus or
        DiplomatVehicle or
        Motorcycle or
        MilitaryVehicle or
        ForeignVehicle;
}
