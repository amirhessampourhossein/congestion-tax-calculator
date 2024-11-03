namespace CongestionTaxCalculator.Exceptions;

public class RuleNotFoundException()
    : Exception("No rules found and tax amount cannot be calculated!");
