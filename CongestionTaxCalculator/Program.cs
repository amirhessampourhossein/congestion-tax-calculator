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

                    Console.WriteLine();
                    ShowAllTaxRecords(dbContext);
                    Console.ReadKey();

                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:

                    AddNewVehicle(dbContext);
                    Console.ReadKey();

                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:

                    AddNewTaxRecord(dbContext, calculator);
                    Console.ReadKey();

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
            .AddScoped<Calculator>()
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
        var vehicles = dbContext.Vehicles
            .AsNoTracking()
            .Include(v => v.TaxRecords)
            .ToList();

        vehicles.Dump();
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

        Console.WriteLine($"\n\nAdded successfully with id : {vehicle.Id}");
    }

    private static void AddNewTaxRecord(AppDbContext dbContext, Calculator calculator)
    {
        Console.Clear();
        Console.Write("Enter vehicle id: ");
        var vehicleId = Console.ReadLine();

        if (!Guid.TryParse(vehicleId, out var vehicleIdAsGuid))
        {
            Console.WriteLine("Invalid vehicle id...");
            return;
        }

        var vehicle = dbContext.Vehicles.Find(vehicleIdAsGuid);

        if (vehicle is null)
        {
            Console.WriteLine("Not Found...");
            return;
        }

        Console.Write("Enter passage dates (csv): ");

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
    }
}

//Use the dates below for testing
//2013-01-14 21:00:00,2013-01-15 21:00:00,2013-02-07 06:23:27,2013-02-07 15:27:00,2013-02-08 06:27:00,2013-02-08 06:20:27,2013-02-08 14:35:00,2013-02-08 15:29:00,2013-02-08 15:47:00,2013-02-08 16:01:00,2013-02-08 16:48:00,2013-02-08 17:49:00,2013-02-08 18:29:00,2013-02-08 18:35:00,2013-03-26 14:25:00,2013-03-28 14:07:27