using CongestionTaxCalculator.Entities.Common;

namespace CongestionTaxCalculator.Entities;

public class Car : IVehicle
{
    public string GetVehicleType()
    {
        return "Car";
    }
}