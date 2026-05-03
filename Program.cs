
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
                Console.WriteLine("\n1. Add Plant");
                Console.WriteLine("2. View Plants");
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
                        return;
                }
            }
        }
    }
}
