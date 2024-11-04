namespace CongestionTaxCalculator.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public VehicleType Type { get; set; }
    public ICollection<TaxRecord> TaxRecords { get; set; } = null!;
}
