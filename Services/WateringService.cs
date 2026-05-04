// WateringService.cs

using PlantCareTracker.Models;

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
        Displays detailed watering reminders.

        Shows last watering date, days since last watering,
        and whether the plant needs water.
        */
        {
            if (!plants.Any())
            {
                Console.WriteLine("No plants available.");
                return;
            }

            Console.WriteLine("\n--- Watering Status ---\n");

            foreach (var plant in plants)
            {
                var last = records
                    .Where(r => r.PlantId == plant.PlantId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                DateTime? lastDate = last?.Date;

                Console.WriteLine($"{plant.Name} ({plant.Location})");

                if (lastDate == null)
                {
                    Console.WriteLine("Last watered: Never");
                    Console.WriteLine("Status: Needs water\n");
                    continue;
                }

                int daysSince = (DateTime.Now - lastDate.Value).Days;

                Console.WriteLine($"Last watered: {daysSince} days ago");

                if (plant.NeedsWater(lastDate))
                {
                    Console.WriteLine("Status: Needs water\n");
                }
                else
                {
                    Console.WriteLine("Status: OK\n");
                }
            }
        }
    }
}