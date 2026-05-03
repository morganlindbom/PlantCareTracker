
using PlantCareTracker.Models;
using PlantCareTracker.Services;

namespace PlantCareTracker
{
    public class Program
    {
        static void Main(string[] args)
        {
            var plantService = new PlantService();

            while (true)
            {
                string title = "Plant Care Tracker";
                Console.WriteLine(  title  );
                
                Console.WriteLine("\n1. Add Plant");
                Console.WriteLine("2. View Plants");
                Console.WriteLine("3. Delete Plant");
                Console.WriteLine("4. Search Plant");
                Console.WriteLine("5. Log Watering");
                Console.WriteLine("6. View Watering");
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
                            PlantId = Guid.NewGuid().ToString(),
                            Name = name,
                            Location = location,
                            WateringDays = days
                        });
                        Console.WriteLine("Plant added");
                        break;

                    case "2":
                        var plants = plantService.GetAllPlants();
                        foreach (var p in plants)
                        {
                            Console.WriteLine($"{p.Name} ({p.PlantId})");

                        }
                        break;

                    case "3":
                        Console.Write("Enter ID: ");
                        if (!plantService.DeletePlant(Console.ReadLine()))
                            Console.WriteLine("Plant not found");
                        break;

                    case "4":
                        Console.Write("Search name: ");
                        var results = plantService.SearchPlant(Console.ReadLine());
                        foreach (var p in results)
                            Console.WriteLine($"{p.Name} ({p.PlantId})");
                        break;

                    case "5":
                        Console.Write("Plant ID: ");
                        string id = Console.ReadLine();

                        Console.Write("Notes: ");
                        string notes = Console.ReadLine();

                        plantService.LogWatering(id, notes);
                        Console.WriteLine("Watering logged!");
                        break;

                    case "6":
                        foreach (var r in plantService.GetAllLogs())
                            Console.WriteLine($"{r.PlantId} - {r.Date}");
                        break;

                    case "7":
                        plantService.ShowReminders();
                        break;

                    case "0":
                        return;
                }
            }
        }
    }
}
