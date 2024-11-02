namespace CongestionTaxCalculator.Services.RulesProvider;

public class CongestionTaxRule
{
    public TimeOnly StartTime { get; set; }

    public TimeOnly FinishTime { get; set; }

    public decimal Amount { get; set; }
}
