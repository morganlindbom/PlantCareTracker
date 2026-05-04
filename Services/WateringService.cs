// WateringService.cs

using PlantCareTracker.Models;
using PlantCareTracker.Utils;

namespace PlantCareTracker.Services
{
    public class WateringService
    {
        private readonly List<WateringRecord> records;
        private readonly List<Plant> plants;

        public WateringService(List<Plant> plants, List<WateringRecord> records)
        {
            this.plants = plants ?? new List<Plant>();
            this.records = records ?? new List<WateringRecord>();
        }

        public bool LogWatering(string plantId, string notes)
        {
            var plant = plants.FirstOrDefault(p => p.PlantId == plantId);

            if (plant == null)
                return false;

            records.Add(new WateringRecord
            {
                PlantId = plantId,
                Date = DateTime.Now,
                Notes = notes
            });

            return true;
        }

        public List<WateringRecord> GetAllLogs()
        {
            return records;
        }

        public void ShowReminders()
        /*
        Displays detailed watering reminders with consistent layout.
        */
        {
            if (!plants.Any())
            {
                Console.WriteLine("No plants available.");
                return;
            }

            const int width = 40;

            Console.WriteLine();
            Console.WriteLine(ConsoleHelper.CenterText("--- Watering Status ---", width));
            Console.WriteLine(new string('-', width));

            foreach (var plant in plants)
            {
                var last = records
                    .Where(r => r.PlantId == plant.PlantId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                DateTime? lastDate = last?.Date;

                Console.WriteLine(ConsoleHelper.CenterText($"{plant.Name} ({plant.Location})", width));

                if (lastDate == null)
                {
                    Console.WriteLine(ConsoleHelper.CenterText("Last watered: Never", width));
                    Console.WriteLine(ConsoleHelper.CenterText("Status: Needs water 💧", width));
                    Console.WriteLine(new string('-', width));
                    continue;
                }

                int daysSince = (DateTime.Now - lastDate.Value).Days;
                string dayText = daysSince == 1 ? "day" : "days";

                Console.WriteLine(ConsoleHelper.CenterText($"Last watered: {daysSince} {dayText} ago", width));

                if (plant.NeedsWater(lastDate))
                {
                    Console.WriteLine(ConsoleHelper.CenterText("Status: Needs water 💧", width));
                }
                else
                {
                    Console.WriteLine(ConsoleHelper.CenterText("Status: OK 👍", width));
                }

                Console.WriteLine(new string('-', width));
            }

            Console.WriteLine();
        }
    }
}