// Program.cs

using PlantCareTracker.Models;
using PlantCareTracker.Services;
using System.IO;
using System.Text.Json;

namespace PlantCareTracker
{
    public class Program
    {
        static void Main(string[] args)
        {
            string path = Path.Combine("Data", "seed-data.json");

            List<Plant> plants = new();
            List<WateringRecord> records = new();

            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    plants = JsonSerializer.Deserialize<List<Plant>>(
                        data.GetProperty("plants").GetRawText(),
                        options
                    ) ?? new List<Plant>();

                    records = JsonSerializer.Deserialize<List<WateringRecord>>(
                        data.GetProperty("wateringRecords").GetRawText(),
                        options
                    ) ?? new List<WateringRecord>();
                }
                catch
                {
                    Console.WriteLine("Error reading data file. Starting with empty lists.");
                    plants = new List<Plant>();
                    records = new List<WateringRecord>();
                }
            }
            else
            {
                Console.WriteLine("Data file not found. Starting with empty lists.");
            }

            var plantService = new PlantService(plants);
            var wateringService = new WateringService(plants, records);

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Plant Care Tracker");

                Console.WriteLine("\n1. Add Plant");
                Console.WriteLine("2. View Plants");
                Console.WriteLine("3. Delete Plant");
                Console.WriteLine("4. Search Plant");
                Console.WriteLine("5. Log Watering");
                Console.WriteLine("6. View Watering Logs");
                Console.WriteLine("7. Reminders");
                Console.WriteLine("0 Exit");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Console.Write("Name: ");
                        string name = Console.ReadLine();

                        Console.Write("Location: ");
                        string location = Console.ReadLine();

                        Console.Write("Watering days: ");
                        if (!int.TryParse(Console.ReadLine(), out int days) || days < 1)
                        {
                            Console.WriteLine("\nInvalid input\n");
                            break;
                        }

                        plantService.AddPlant(new Plant
                        {
                            Name = name,
                            Location = location,
                            WateringDays = days
                        });

                        Console.WriteLine("\nPlant added\n");
                        break;

                    case "2":
                        var plantList = plantService.GetAllPlants();

                        if (!plantList.Any())
                        {
                            Console.WriteLine("\nNo plants found\n");
                            break;
                        }

                        Console.WriteLine();

                        foreach (var p in plantList)
                        {
                            Console.WriteLine($"{p.PlantId} | {p.Name} | {p.Location} | Every {p.WateringDays} days");
                        }

                        Console.WriteLine();
                        break;

                    case "3":
                        Console.Write("Enter ID: ");
                        string deleteId = Console.ReadLine();

                        if (!plantService.DeletePlant(deleteId))
                            Console.WriteLine("\nPlant not found\n");
                        else
                            Console.WriteLine("\nPlant deleted\n");

                        break;

                    case "4":
                        Console.Write("Search name: ");
                        string search = Console.ReadLine();

                        var results = plantService.SearchPlant(search);

                        if (!results.Any())
                        {
                            Console.WriteLine("\nNo matching plants found\n");
                            break;
                        }

                        Console.WriteLine();

                        foreach (var p in results)
                        {
                            Console.WriteLine($"{p.PlantId} | {p.Name} | {p.Location} | Every {p.WateringDays} days");
                        }

                        Console.WriteLine();
                        break;

                    case "5":
                        Console.Write("Plant ID: ");
                        string id = Console.ReadLine();

                        Console.Write("Notes: ");
                        string notes = Console.ReadLine();

                        if (wateringService.LogWatering(id, notes))
                            Console.WriteLine("\nWatering logged!\n");
                        else
                            Console.WriteLine("\nPlant not found\n");

                        break;

                    case "6":
                        var logs = wateringService.GetAllLogs();

                        if (!logs.Any())
                        {
                            Console.WriteLine("\nNo watering records found\n");
                            break;
                        }

                        Console.WriteLine();

                        foreach (var r in logs)
                        {
                            Console.WriteLine($"{r.PlantId}");
                            Console.WriteLine($"Date: {r.Date}");
                            Console.WriteLine($"Notes: {r.Notes}\n");
                        }

                        break;

                    case "7":
                        Console.WriteLine();
                        wateringService.ShowReminders();
                        Console.WriteLine();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("\nInvalid choice\n");
                        break;
                }
            }
        }
    }
}