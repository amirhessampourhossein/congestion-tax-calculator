namespace CongestionTaxCalculator.Exceptions;

public class CongestionTaxRuleNotFoundException()
    : Exception("No rules found and tax amount cannot be calculated!");
