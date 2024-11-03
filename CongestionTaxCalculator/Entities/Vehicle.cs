namespace CongestionTaxCalculator.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public VehicleType Type { get; set; }
    public ICollection<TaxRecord> TaxRecords { get; set; } = null!;

    public bool IsTaxExmept()
    {
        return Type is
            VehicleType.Motorcycle or
            VehicleType.Bus or
            VehicleType.Diplomat or
            VehicleType.Emergency or
            VehicleType.Foregin or
            VehicleType.Military;
    }
}
