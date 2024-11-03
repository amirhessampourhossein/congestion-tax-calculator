using CongestionTaxCalculator.Exceptions;
using CongestionTaxCalculator.Services.Json;

namespace CongestionTaxCalculator.Services.RulesProvider;

public class GothenburgCongestionTaxRulesProvider : ICongestionTaxRulesProvider
{
    private const string GothenburgRulesJsonPath = "ExternalRules/Gothenburg.json";

    public List<CongestionTaxRule>? GetRules()
    {
        try
        {
            var rulesJson = File.ReadAllText(GothenburgRulesJsonPath);

            var rules = JsonService.Deserialize<List<CongestionTaxRule>>(rulesJson);

            return rules;
        }
        catch (Exception ex)
        {
            throw new RulesProviderException(ex);
        }
    }
}
