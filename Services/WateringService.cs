// WateringService.cs

using System;
using System.Collections.Generic;
using System.Linq;
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
        /*
        Logs a watering event for a plant.

        Searches for the plant using its ID and creates a new
        WateringRecord with the current timestamp.

        Returns false if the plant does not exist.
        */
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
        /*
        Returns all watering records.

        In a larger system, this could return a copy instead
        of the original list to prevent modification.
        */
        {
            return records;
        }

        public void ShowReminders()
        /*
        Displays watering status for all plants.

        For each plant:
        - Finds last watering date
        - Calculates days since watering
        - Uses NeedsWater() to determine status
        - Outputs formatted UI
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

        public Plant? GetMostRecentlyWateredPlant()
        /*
        Returns the most recently watered plant.

        Finds the latest WateringRecord and maps it
        back to the corresponding Plant.
        */
        {
            if (!records.Any())
                return null;

            var latestRecord = records
                .OrderByDescending(r => r.Date)
                .First();

            return plants.FirstOrDefault(p => p.PlantId == latestRecord.PlantId);
        }

        public Plant? GetNextPlantToWater()
        /*
        Returns the plant closest to its next watering time.

        Uses the next watering date for each plant and compares the time
        distance to now. Overdue plants are treated as due now so they do
        not dominate the result with extreme overdue bias.
        */
        {
            if (!plants.Any())
                return null;

            var now = DateTime.Now;

            var plantData = plants.Select(p =>
            {
                var last = records
                    .Where(r => r.PlantId == p.PlantId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                var nextDate = last == null ? now : last.Date.AddDays(p.WateringDays);
                var timeUntilNext = nextDate > now ? nextDate - now : TimeSpan.Zero;

                return new
                {
                    Plant = p,
                    NextDate = nextDate,
                    TimeUntilNext = timeUntilNext
                };
            });

            return plantData
                .OrderBy(x => x.TimeUntilNext)
                .ThenBy(x => x.NextDate)
                .ThenBy(x => x.Plant.PlantId)
                .Select(x => x.Plant)
                .FirstOrDefault();
        }
    }
}