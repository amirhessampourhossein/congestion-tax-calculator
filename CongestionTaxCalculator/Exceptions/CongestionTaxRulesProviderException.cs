namespace CongestionTaxCalculator.Exceptions;

public class CongestionTaxRulesProviderException(Exception innerException)
    : Exception(
        "Failed to read the congestion tax rules!",
        innerException);
