using CongestionTaxCalculator.Data;
using CongestionTaxCalculator.Entities;
using CongestionTaxCalculator.Exceptions;
using CongestionTaxCalculator.Services.RulesProvider;
using PublicHoliday;
using System.Data;

namespace CongestionTaxCalculator.Services;

public class Calculator(
    AppDbContext dbContext,
    ICongestionTaxRulesProvider congestionTaxRulesProvider,
    SwedenPublicHoliday swedenPublicHoliday)
{
    private const int Year = 2013;
    private const int JulyMonthNumber = 7;
    private const decimal MaxCongestionTaxAmount = 60m;

    public decimal CalculateTax(Vehicle vehicle, DateTime[] dates)
    {
        if (vehicle.IsTaxExmept())
            return 0;

        dates = FilterTollFreeDays(dates);

         if (dates.Length == 0)
            return 0;

        var congestionTaxRules = congestionTaxRulesProvider.GetRules();

        if (congestionTaxRules is not { Count: not 0 })
            throw new CongestionTaxRuleNotFoundException();

        decimal finalTaxAmount = default;
        decimal finalTaxAmountForCurrentDay = default;
        var currentDay = dates[0].DayOfYear;

        for (int i = 0; i < dates.Length; i++)
        {
            var chargeRule = SearchRulesByPassageDate(congestionTaxRules, dates[i]);
            int lastIndexWithinHour = i;

            for (int j = i + 1; j < dates.Length; j++)
            {
                if (dates[j] - dates[i] <= TimeSpan.FromMinutes(60))
                {
                    lastIndexWithinHour = j;

                    var matchedRule = SearchRulesByPassageDate(congestionTaxRules, dates[j]);

                    if (matchedRule.Amount > chargeRule.Amount)
                        chargeRule = matchedRule;
                }
                else break;
            }

            i = lastIndexWithinHour;

            if (dates[i].DayOfYear != currentDay)
            {
                currentDay = dates[i].DayOfYear;
                finalTaxAmount += finalTaxAmountForCurrentDay;
                finalTaxAmountForCurrentDay = chargeRule.Amount;
            }
            else
            {
                finalTaxAmountForCurrentDay += chargeRule.Amount;
                if (finalTaxAmountForCurrentDay > MaxCongestionTaxAmount)
                    finalTaxAmountForCurrentDay = MaxCongestionTaxAmount;
            }
        }

        finalTaxAmount += finalTaxAmountForCurrentDay;

        dbContext.TaxRecords.Add(new()
        {
            PassageDates = string.Join(",", dates),
            VehicleId = vehicle.Id,
            TotalTax = finalTaxAmount
        });

        dbContext.SaveChanges();

        return finalTaxAmount;
    }

    private DateTime[] FilterTollFreeDays(DateTime[] dates)
    {
        var freeDays = GetTollFreeDays();

        var filteredDates = dates
            .Where(d =>
                d.Month != JulyMonthNumber &&
                !d.DayOfWeek.IsWeekend() &&
                !freeDays.Contains(d.DayOfYear))
            .ToArray();

        return filteredDates;
    }

    private int[] GetTollFreeDays()
    {
        return swedenPublicHoliday
            .PublicHolidays(Year)
            .SelectMany(d =>
            new int[]
            {
                d.DayOfYear - 1,
                d.DayOfYear,
            })
            .Where(d => d != 0)
            .Distinct()
            .ToArray();
    }

    private static CongestionTaxRule SearchRulesByPassageDate(
        List<CongestionTaxRule> rules,
        DateTime date)
    {
        var matchedRule = rules
            .SingleOrDefault(r => TimeOnly.FromDateTime(date).IsBetweenInclusive(r.StartTime, r.FinishTime))
            ?? throw new CongestionTaxRuleNotFoundException();

        return matchedRule;
    }
}
