using CongestionTaxCalculator.Data;
using CongestionTaxCalculator.Entities;
using CongestionTaxCalculator.Services;
using CongestionTaxCalculator.Services.RulesProvider;
using Dumpify;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PublicHoliday;
using System.Globalization;

namespace CongestionTaxCalculator;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Title = nameof(CongestionTaxCalculator);
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        var serviceProvider = BuildServiceProvider();

        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
        var calculator = serviceProvider.GetRequiredService<Calculator>();

        dbContext.Database.EnsureCreated();

        while (true)
        {
            DisplayMainMenu();
            var selection = Console.ReadKey();

            switch (selection.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:

                    ShowAllTaxRecords(dbContext);

                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:

                    AddNewVehicle(dbContext);

                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:

                    AddNewTaxRecord(dbContext, calculator);

                    break;
                default: break;
            }

            Console.Clear();
        }
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var serviceProvider = new ServiceCollection()
            .AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source=congestion-tax.db"))
            .AddSingleton<ICongestionTaxRulesProvider, GothenburgCongestionTaxRulesProvider>()
            .AddSingleton<SwedenPublicHoliday>()
            .AddSingleton<Calculator>()
            .BuildServiceProvider();

        return serviceProvider;
    }

    private static void DisplayMainMenu()
    {
        Console.WriteLine("1.Show all tax records");
        Console.WriteLine("2.Add a vehicle");
        Console.WriteLine("3.Calculate tax");
        Console.Write("\nSelect: ");
    }

    private static void DisplayVehicleTypeMenu()
    {
        Console.WriteLine("1.Car");
        Console.WriteLine("2.Motorcycle");
        Console.WriteLine("3.Bus");
        Console.WriteLine("4.Diplomat");
        Console.WriteLine("4.Emergency");
        Console.WriteLine("4.Foregin");
        Console.WriteLine("5.Military");
        Console.Write("\nSelect: ");
    }

    private static void ShowAllTaxRecords(AppDbContext dbContext)
    {
        Console.WriteLine();

        var vehicles = dbContext.Vehicles
            .AsNoTracking()
            .Include(v => v.TaxRecords)
            .ToList();

        foreach (var vehicle in vehicles)
        {
            var totalTaxAmount = vehicle.TaxRecords.Sum(r => r.TaxAmount);
            Console.WriteLine($"Total tax amount for vehicle '{vehicle.Id}' is {totalTaxAmount} SEK");
        }

        vehicles.Dump();

        Console.ReadKey();
    }

    private static void AddNewVehicle(AppDbContext dbContext)
    {
        Console.Clear();
        DisplayVehicleTypeMenu();

        var vehicleTypeSelection = Console.ReadKey().KeyChar.ToString();
        if (!Enum.TryParse<VehicleType>(vehicleTypeSelection, out var vehicleType))
        {
            Console.WriteLine("Invalid vehicle type!");
            return;
        }

        var vehicle = new Vehicle
        {
            Type = vehicleType,
        };

        dbContext.Vehicles.Add(vehicle);
        dbContext.SaveChanges();

        Console.WriteLine("\n\nAdded successfully...");
        Console.ReadKey();
    }

    private static void AddNewTaxRecord(AppDbContext dbContext, Calculator calculator)
    {
        Console.Clear();
        Console.Write("Enter vehicle id: ");
        var vehicleId = Console.ReadLine();

        var vehicle = dbContext.Vehicles.Find(Guid.Parse(vehicleId!));

        if (vehicle is null)
        {
            Console.WriteLine("Not Found...");
            return;
        }

        Console.Write("\nEnter passage dates (csv): ");

        DateTime[] passageDates;
        try
        {
            passageDates = [.. Console.ReadLine()!
                .Split(',')
                .Select(DateTime.Parse)
                .Order()];
        }
        catch (Exception)
        {
            Console.WriteLine("Dates are invalid!");
            return;
        }

        calculator.CalculateTax(vehicle, passageDates);

        Console.WriteLine("\n\nFinished...");
        Console.ReadKey();
    }
}

//Use the dates below for testing
//2013-01-14 21:00:00,2013-01-15 21:00:00,2013-02-07 06:23:27,2013-02-07 15:27:00,2013-02-08 06:27:00,2013-02-08 06:20:27,2013-02-08 14:35:00,2013-02-08 15:29:00,2013-02-08 15:47:00,2013-02-08 16:01:00,2013-02-08 16:48:00,2013-02-08 17:49:00,2013-02-08 18:29:00,2013-02-08 18:35:00,2013-03-26 14:25:00,2013-03-28 14:07:27