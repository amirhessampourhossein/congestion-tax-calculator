namespace CongestionTaxCalculator.Entities;

public class TaxRecord
{
    public Guid Id { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public Guid VehicleId { get; set; }
    public DateTime PassageDate { get; set; }
    public decimal TaxAmount { get; set; }
}
