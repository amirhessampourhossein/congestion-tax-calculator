namespace CongestionTaxCalculator.Exceptions;

public class RulesProviderException(Exception innerException)
    : Exception(
        "Failed to read the congestion tax rules!",
        innerException);
