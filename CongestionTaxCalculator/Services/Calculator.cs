using CongestionTaxCalculator.Entities.Common;
using CongestionTaxCalculator.Exceptions;
using CongestionTaxCalculator.Services.RulesProvider;
using System.Data;

namespace CongestionTaxCalculator.Services;

public class Calculator
{
    private const int JulyMonthNumber = 7;
    private const decimal MaxCongestionTaxAmount = 60m;

    public int[] Holidays { get; init; } = null!;
    public ICongestionTaxRulesProvider CongestionTaxRulesProvider { get; init; } = null!;

    public decimal CalculateTax(Vehicle vehicle, DateTime[] dates)
    {
        if (vehicle.IsExemptVehicle())
            return 0;

        dates = FilterTollFreeDays(dates);

        if (dates.Length == 0)
            return 0;

        var congestionTaxRules = CongestionTaxRulesProvider.GetRules();

        if (congestionTaxRules is not { Count: not 0 })
            throw new CongestionTaxRuleNotFoundException();

        decimal finalTaxAmount = default;
        decimal finalTaxAmountForCurrentDay = default;
        var currentDay = dates[0].DayOfYear;

        for (int i = 0; i < dates.Length; i++)
        {
            var chargeRule = SearchRulesByPassageDate(congestionTaxRules, dates[i]);

            for (int j = i + 1; j < dates.Length; j++)
            {
                if (dates[j] - dates[i] <= TimeSpan.FromMinutes(60))
                {
                    var matchedRule = SearchRulesByPassageDate(congestionTaxRules, dates[j]);

                    if (matchedRule.Amount > chargeRule.Amount)
                        chargeRule = matchedRule;
                }
                else break;
            }

            if (dates[i].DayOfYear != currentDay)
            {
                finalTaxAmount += finalTaxAmountForCurrentDay > MaxCongestionTaxAmount
                    ? MaxCongestionTaxAmount
                    : finalTaxAmountForCurrentDay;

                finalTaxAmountForCurrentDay = chargeRule.Amount;

                currentDay = dates[i].DayOfYear;
            }
            else
                finalTaxAmountForCurrentDay += chargeRule.Amount;
        }

        return finalTaxAmount;
    }

    private DateTime[] FilterTollFreeDays(DateTime[] dates)
    {
        var filteredDates = dates
            .Where(d =>
                d.Month != JulyMonthNumber &&
                !d.DayOfWeek.IsWeekend() &&
                !Holidays.Contains(d.DayOfYear))
            .ToArray();

        return filteredDates;
    }

    private static CongestionTaxRule SearchRulesByPassageDate(List<CongestionTaxRule> rules, DateTime date)
    {
        var matchedRule = rules
            .SingleOrDefault(r => TimeOnly.FromDateTime(date).IsBetweenInclusive(r.StartTime, r.FinishTime))
            ?? throw new CongestionTaxRuleNotFoundException();

        return matchedRule;
    }
}
