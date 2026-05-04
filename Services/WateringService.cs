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
        Displays watering reminders for all plants.

        Uses Plant.NeedsWater() to determine if watering is required.
        This keeps business logic inside the Plant model.
        */
        {
            foreach (var plant in plants)
            {
                var last = records
                    .Where(r => r.PlantId == plant.PlantId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                DateTime? lastDate = last?.Date;

                if (lastDate == null)
                {
                    Console.WriteLine($"{plant.Name}: Never watered");
                    continue;
                }

                if (plant.NeedsWater(lastDate))
                {
                    int daysSince = (DateTime.Now - lastDate.Value).Days;
                    Console.WriteLine($"{plant.Name}: Needs water! ({daysSince} days since last)");
                }
            }
        }
    }
}