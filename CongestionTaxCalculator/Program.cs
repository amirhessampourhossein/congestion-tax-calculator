using CongestionTaxCalculator.Services;
using CongestionTaxCalculator.Services.RulesProvider;
using Microsoft.Extensions.DependencyInjection;
using PublicHoliday;

namespace CongestionTaxCalculator;

internal class Program
{
    private const int Year = 2013;

    static void Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();

        var testDates = new List<string>()
        {
            "2013-01-14 21:00:00",
            "2013-01-15 21:00:00",
            "2013-02-07 06:23:27",
            "2013-02-07 15:27:00",
            "2013-02-08 06:27:00",
            "2013-02-08 06:20:27",
            "2013-02-08 14:35:00",
            "2013-02-08 15:29:00",
            "2013-02-08 15:47:00",
            "2013-02-08 16:01:00",
            "2013-02-08 16:48:00",
            "2013-02-08 17:49:00",
            "2013-02-08 18:29:00",
            "2013-02-08 18:35:00",
            "2013-03-26 14:25:00",
            "2013-03-28 14:07:27"
        }
        .Select(DateTime.Parse)
        .ToList();

        var calculator = serviceProvider.GetRequiredService<Calculator>();
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(new Calculator()
            {
                CongestionTaxRulesProvider = new GothenburgCongestionTaxRulesProvider(),
                Holidays = new SwedenPublicHoliday()
                    .PublicHolidays(Year)
                    .SelectMany(d =>
                        new int[]
                        {
                            d.DayOfYear - 1,
                            d.DayOfYear,
                        })
                    .Distinct()
                    .ToArray()
            })
            .BuildServiceProvider();

        return serviceProvider;
    }
}
