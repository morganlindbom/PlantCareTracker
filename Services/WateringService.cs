
// WateringService.cs

using PlantCareTracker.Models;

namespace PlantCareTracker.Services
{
    public class WateringService
    {
        private readonly List<WateringRecord> records = new();
        private readonly List<Plant> plants;

        public WateringService(List<Plant> plants)
        {
            this.plants = plants;
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
        {
            foreach (var plant in plants)
            {
                var last = records
                    .Where(r => r.PlantId == plant.PlantId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                if (last == null)
                {
                    Console.WriteLine($"{plant.Name}: Never watered");
                    continue;
                }

                int days = (DateTime.Now - last.Date).Days;

                if (days >= plant.WateringDays)
                {
                    Console.WriteLine($"{plant.Name}: Needs water!");
                }
            }
        }
    }
}

