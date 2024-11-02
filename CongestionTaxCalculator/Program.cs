using Microsoft.Extensions.DependencyInjection;
using PublicHoliday;

namespace CongestionTaxCalculator;

internal class Program
{
    private const int Year = 2013;

    static void Main(string[] args)
    {
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton(new Calculator()
            {
                HolidayDays = new SwedenPublicHoliday()
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
