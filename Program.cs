// Program.cs

using PlantCareTracker.Models;
using PlantCareTracker.Services;
using PlantCareTracker.Utils;
using System.IO;
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

            try
            {
                plantService.AddPlant(name, location, days);
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

            // ✔ HEADER (ingen p här!)
            Console.WriteLine($"{"ID",-10} | {ConsoleHelper.CenterText("Name", 12)} | {ConsoleHelper.CenterText("Location", 12)} | Watering");
            Console.WriteLine(new string('-', 60));

            foreach (var p in plants)
            {
                Console.WriteLine(
                    $"{p.PlantId,-10} | {ConsoleHelper.CenterText(p.Name, 12)} | {ConsoleHelper.CenterText(p.Location, 12)} | Every {p.WateringDays} days"
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
    }
}