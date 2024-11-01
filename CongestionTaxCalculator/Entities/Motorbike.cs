using CongestionTaxCalculator.Entities.Common;

namespace CongestionTaxCalculator.Entities;

public class Motorbike : IVehicle
{
    public string GetVehicleType()
    {
        return "Motorbike";
    }
}