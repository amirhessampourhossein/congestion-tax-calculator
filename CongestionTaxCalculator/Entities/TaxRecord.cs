namespace CongestionTaxCalculator.Entities;

public class TaxRecord
{
    public Guid Id { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public Guid VehicleId { get; set; }
    public string PassageDates { get; set; } = null!;
    public decimal TotalTax { get; set; }
    public DateTime RecordDate { get; set; }
}
