// Program.cs

using PlantCareTracker.Models;
using PlantCareTracker.Services;
using PlantCareTracker.Utils;
using System.IO;
using System.Numerics;
using System.Text.Json;

namespace PlantCareTracker
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string path = Path.Combine("Data", "seed-data.json");

            var dataService = new DataService();

            var (plants, records) = dataService.LoadData(path);

            var plantService = new PlantService(plants);
            var wateringService = new WateringService(plants, records);

            while (true)
            {
                ShowMenu();
                HandleUserChoice(GetUserChoice(), plantService, wateringService);

            }
        }
        //*****************************************************************************
        // ShowMenu()

        static void ShowMenu()
        /*
        Displays the main menu in the console.

        This function prints all available user options.
        It does not return anything and is called from Main().
        */
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
            Console.WriteLine("8. Update plant health");
            Console.WriteLine("9. View struggling plants");
            Console.WriteLine("10. Most recently watered plant");
            Console.WriteLine("11. Next plant to water");
            Console.WriteLine("12. Average watering interval");
            Console.WriteLine("0 Exit");
        }
        //*****************************************************************************
        // GetUserChoice()

        static string GetUserChoice()
        /*
        Reads the user's menu choice from the console.

        This function waits for user input and returns it as a string.
        It does not validate the input, only retrieves it.
        */
        {
            Console.Write("\nSelect option: ");
            return Console.ReadLine();
        }
        //*****************************************************************************
        // HandleUserChoice()

        static void HandleUserChoice(
            string input,
            PlantService plantService,
            WateringService wateringService
        )
        /*
        Handles the user's menu selection.

        This function receives the user's input and calls the correct
        method based on the selected option.

        It does not return anything, it only executes actions.
        */
        {
            switch (input)
            {
                case "1":
                    AddPlant(plantService);
                    break;

                case "2":
                    ViewPlants(plantService);
                    break;

                case "3":
                    DeletePlant(plantService);
                    break;

                case "4":
                    SearchPlant(plantService);
                    break;

                case "5":
                    LogWatering(wateringService);
                    break;

                case "6":
                    ViewLogs(wateringService);
                    break;

                case "7":
                    ShowReminders(wateringService);
                    break;

                case "8":
                    UpdatePlantHealth(plantService);
                    break;

                case "9":
                    ViewStrugglingPlants(plantService);
                    break;

                case "10":
                    ShowMostRecentlyWatered(wateringService);
                    break;

                case "11":
                    ShowNextPlantToWater(wateringService);
                    break;

                case "12":
                    ShowAverageWatering(wateringService);
                    break;

                case "0":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("\nInvalid choice\n");
                    break;
            }
        }
        //*****************************************************************************
        // AddPlant()

        static void AddPlant(PlantService plantService)
        /*
        Collects validated input and sends it to the service.
        */
        {
            string name = ConsoleHelper.ReadRequiredInput("Name");
            string location = ConsoleHelper.ReadRequiredInput("Location");
            int days = ConsoleHelper.ReadIntInput("Watering days");
            string type = ConsoleHelper.ReadRequiredInput("Type");

            try
            {
                plantService.AddPlant(name, location, days, type);

                Console.WriteLine("\nPlant added\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}\n");
            }
        }
        //*****************************************************************************
        // ViewPlants()

        static void ViewPlants(PlantService plantService)
        /*
        Displays all plants in a formatted table.

        Uses column alignment to improve readability.
        */
        {
            var plants = plantService.GetAllPlants();

            if (!plants.Any())
            {
                Console.WriteLine("\nNo plants found\n");
                return;
            }

            Console.WriteLine();

            // Header with centered text
            Console.WriteLine(
    $"{ConsoleHelper.CenterText("ID", 15)} | {ConsoleHelper.CenterText("Name", 12)} | {ConsoleHelper.CenterText("Location", 12)} | {ConsoleHelper.CenterText("Type", 10)} | {ConsoleHelper.CenterText("Health", 12)} | {ConsoleHelper.CenterText("Watering", 20)} |"
);
            Console.WriteLine(new string('-', 98));

            foreach (var p in plants)
            {
                string type = string.IsNullOrWhiteSpace(p.Type) ? "N/A" : p.Type;

                Console.WriteLine(
                    $"{ConsoleHelper.CenterText(p.PlantId, 15)} | " +
                    $"{ConsoleHelper.CenterText(p.Name, 12)} | " +
                    $"{ConsoleHelper.CenterText(p.Location, 12)} | " +
                    $"{ConsoleHelper.CenterText(type, 10)} | " +
                    $"{ConsoleHelper.CenterText(p.HealthStatus.ToString(), 12)} | " +
                    $"{ConsoleHelper.CenterText($"Every {p.WateringDays} days", 20)} |"
                );
            }

            Console.WriteLine();
        }
        //*****************************************************************************
        // DeletePlant()

        static void DeletePlant(PlantService plantService)
        /*
        Deletes a plant by ID.

        Prompts the user for an ID and attempts to remove
        the plant from the system.
        */
        {
            Console.Write("Enter ID: ");
            string id = Console.ReadLine();

            if (!plantService.DeletePlant(id))
                Console.WriteLine("\nPlant not found\n");
            else
                Console.WriteLine("\nPlant deleted\n");
        }
        //*****************************************************************************
        // SearchPlant()

        static void SearchPlant(PlantService plantService)
        /*
        Searches for plants by name.

        Allows partial matching and prints all matching results.
        */
        {
            Console.Write("Search name: ");
            string search = Console.ReadLine();

            var results = plantService.SearchPlant(search);

            if (!results.Any())
            {
                Console.WriteLine("\nNo matching plants found\n");
                return;
            }

            Console.WriteLine();

            foreach (var p in results)
            {
                Console.WriteLine($"{p.PlantId} | {p.Name} | {p.Location} | Every {p.WateringDays} days");
            }

            Console.WriteLine();
        }
        //*****************************************************************************
        // LogWatering()

        static void LogWatering(WateringService wateringService)
        /*
        Logs a watering event for a plant.

        Checks if the plant exists and adds a watering record
        with the current date and user notes.
        */
        {
            Console.Write("Plant ID: ");
            string id = Console.ReadLine();

            Console.Write("Notes: ");
            string notes = Console.ReadLine();

            if (wateringService.LogWatering(id, notes))
                Console.WriteLine("\nWatering logged!\n");
            else
                Console.WriteLine("\nPlant not found\n");
        }
        //*****************************************************************************
        // ViewLogs()

        static void ViewLogs(WateringService wateringService)
        /*
        Displays all watering records.

        Prints plant ID, date, and notes for each record.
        */
        {
            var logs = wateringService.GetAllLogs();

            if (!logs.Any())
            {
                Console.WriteLine("\nNo watering records found\n");
                return;
            }

            Console.WriteLine();

            foreach (var r in logs)
            {
                Console.WriteLine($"{r.PlantId}");
                Console.WriteLine($"Date: {r.Date}");
                Console.WriteLine($"Notes: {r.Notes}\n");
            }
        }
        //*****************************************************************************
        // ShowReminders()

        static void ShowReminders(WateringService wateringService)
        /*
        Displays watering reminders.

        Checks all plants and prints those that need watering
        based on last watering date and interval.
        */
        {
            Console.WriteLine();
            wateringService.ShowReminders();
            Console.WriteLine();
        }
        //*****************************************************************************
        // UpdatePlantHealth()

        static void UpdatePlantHealth(PlantService plantService)
        /*
        Allows user to update plant health status.
        */
        {
            Console.Write("Enter Plant ID: ");
            string id = Console.ReadLine();

            Console.WriteLine("Select health status:");
            Console.WriteLine("1. Healthy");
            Console.WriteLine("2. Struggling");
            Console.WriteLine("3. Thriving");

            string choice = Console.ReadLine();

            HealthStatus status;

            switch (choice)
            {
                case "1":
                    status = HealthStatus.Healthy;
                    break;
                case "2":
                    status = HealthStatus.Struggling;
                    break;
                case "3":
                    status = HealthStatus.Thriving;
                    break;
                default:
                    Console.WriteLine("Invalid choice");
                    return;
            }

            if (plantService.UpdateHealth(id, status))
                Console.WriteLine("\nHealth updated\n");
            else
                Console.WriteLine("\nPlant not found\n");
        }
        //*****************************************************************************
        // ViewStrugglingPlants()

        static void ViewStrugglingPlants(PlantService plantService)
        /*
        Displays all plants with "Struggling" health status.
        */
        {
            var plants = plantService.GetStrugglingPlants();

            if (!plants.Any())
            {
                Console.WriteLine();
                Console.WriteLine($"{ConsoleHelper.CenterText("(((No struggling plants)))", 37)}");
                return;
            }
            Console.WriteLine();
            Console.WriteLine($"{ConsoleHelper.CenterText("--- Struggling Plants ---", 37)}");
            Console.WriteLine();
            foreach (var p in plants)
            {
                Console.WriteLine($"{ConsoleHelper.CenterText(p.PlantId,10)} | " +
                                  $"{ConsoleHelper.CenterText(p.Name, 12)} | " +
                                  $"{ConsoleHelper.CenterText(p.Location, 12)} |");
            }

            Console.WriteLine();
        }
        //*****************************************************************************
        // ShowMostRecentlyWatered()

        static void ShowMostRecentlyWatered(WateringService wateringService)
        /*
        Displays the most recently watered plant.
        */
        {
            var plant = wateringService.GetMostRecentlyWateredPlant();

            if (plant == null)
            {
                Console.WriteLine("\nNo watering records found\n");
                return;
            }

            const int width = 37;

            Console.WriteLine();
            Console.WriteLine($"{ConsoleHelper.CenterText("--- Most Recently Watered ---", width)}");
            Console.WriteLine();

            Console.WriteLine(
                $"{ConsoleHelper.CenterText(plant.PlantId, 10)} | " +
                $"{ConsoleHelper.CenterText(plant.Name, 12)} | " +
                $"{ConsoleHelper.CenterText(plant.Location, 12)} |"
            );

            Console.WriteLine();
        }
        //*****************************************************************************
        //*****************************************************************************
        // ShowNextPlantToWater()

        static void ShowNextPlantToWater(WateringService wateringService)
        /*
        Displays the plant that should be watered next.
        */
        {
            var plant = wateringService.GetNextPlantToWater();

            if (plant == null)
            {
                Console.WriteLine("\nNo plants available\n");
                return;
            }

            const int width = 37;

            Console.WriteLine();
            Console.WriteLine($"{ConsoleHelper.CenterText("--- Next Plant To Water ---", width)}");
            Console.WriteLine();

            Console.WriteLine(
                $"{ConsoleHelper.CenterText(plant.PlantId, 10)} | " +
                $"{ConsoleHelper.CenterText(plant.Name, 12)} | " +
                $"{ConsoleHelper.CenterText(plant.Location, 12)} |"
            );

            Console.WriteLine();
        }
        //*****************************************************************************
        // ShowAverageWatering()

        static void ShowAverageWatering(WateringService wateringService)
        /*
        Displays the average watering interval.
        */
        {
            double avg = wateringService.GetAverageWateringInterval();
            const int width = 37;

            Console.WriteLine();
            Console.WriteLine($"{ConsoleHelper.CenterText("--- Average Watering Interval ---", width)}");
            Console.WriteLine();

            Console.WriteLine(
                ConsoleHelper.CenterText(
                    $"Average: {avg.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)} days",
                    width
                )
            );

            Console.WriteLine();
        }
    }
}