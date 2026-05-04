
// Program.cs

using PlantCareTracker.Models;
using PlantCareTracker.Services;

namespace PlantCareTracker
{
    public class Program
    {
        static void Main(string[] args)
        {
            var plantService = new PlantService();
            var wateringService = new WateringService(plantService.GetAllPlants());

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
                            Console.WriteLine("Invalid input");
                            break;
                        }

                        plantService.AddPlant(new Plant
                        {
                            Name = name,
                            Location = location,
                            WateringDays = days
                        });

                        Console.WriteLine("Plant added");
                        break;

                    case "2":
                        var plants = plantService.GetAllPlants();

                        if (!plants.Any())
                        {
                            Console.WriteLine("No plants found");
                            break;
                        }

                        foreach (var p in plants)
                        {
                            Console.WriteLine($"{p.Name} ({p.PlantId})");
                        }
                        break;

                    case "3":
                        Console.Write("Enter ID: ");
                        string deleteId = Console.ReadLine();

                        if (!plantService.DeletePlant(deleteId))
                            Console.WriteLine("Plant not found");
                        else
                            Console.WriteLine("Plant deleted");

                        break;

                    case "4":
                        Console.Write("Search name: ");
                        string search = Console.ReadLine();

                        var results = plantService.SearchPlant(search);

                        if (!results.Any())
                        {
                            Console.WriteLine("No matching plants found");
                            break;
                        }

                        foreach (var p in results)
                        {
                            Console.WriteLine($"{p.Name} ({p.PlantId})");
                        }
                        break;

                    case "5":
                        Console.Write("Plant ID: ");
                        string id = Console.ReadLine();

                        Console.Write("Notes: ");
                        string notes = Console.ReadLine();

                        if (wateringService.LogWatering(id, notes))
                            Console.WriteLine("Watering logged!");
                        else
                            Console.WriteLine("Plant not found");

                        break;

                    case "6":
                        var logs = wateringService.GetAllLogs();

                        if (!logs.Any())
                        {
                            Console.WriteLine("No watering records found");
                            break;
                        }

                        foreach (var r in logs)
                        {
                            Console.WriteLine($"{r.PlantId}:\n{r.Date}\n");
                        }
                        break;

                    case "7":
                        wateringService.ShowReminders();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
            }
        }
    }
}

