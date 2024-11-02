namespace CongestionTaxCalculator.Services.RulesProvider;

public interface ICongestionTaxRulesProvider
{
    List<CongestionTaxRule>? GetRules();
}
