namespace CongestionTaxCalculator.Exceptions;

public class RuleNotFoundException()
    : Exception("No rules found. Tax amount cannot be calculated!");
