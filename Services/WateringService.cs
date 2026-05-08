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
        Displays advanced watering reminders grouped by urgency.

        Categories:
        - Overdue (> 2 days late)
        - Due Today (0–2 days late)
        - Upcoming (within next 2 days)

        This improves usability by helping the user prioritize actions.
        */
        {
            if (!plants.Any())
            {
                Console.WriteLine(ConsoleHelper.CenterText("No plants available.", 40));
                return;
            }

            const int width = 40;
            var now = DateTime.Now;

            var overdue = new List<Plant>();
            var dueToday = new List<Plant>();
            var upcoming = new List<Plant>();

            foreach (var plant in plants)
            {
                var last = records
                    .Where(r => r.PlantId == plant.PlantId)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                if (last == null)
                {
                    // Never watered → treat as overdue
                    overdue.Add(plant);
                    continue;
                }

                var nextDate = last.Date.AddDays(plant.WateringDays);
                var diff = (now - nextDate).TotalDays;

                if (diff > 2)
                {
                    overdue.Add(plant);
                }
                else if (diff >= 0)
                {
                    dueToday.Add(plant);
                }
                else if (diff >= -2)
                {
                    upcoming.Add(plant);
                }
            }

            Console.WriteLine();
            Console.WriteLine(ConsoleHelper.CenterText("--- Overdue ---", width));
            Console.WriteLine(new string('-', width));

            foreach (var p in overdue)
            {
                Console.WriteLine(ConsoleHelper.CenterText($"{p.PlantId} | {p.Name} | {p.Location}", width));
            }

            Console.WriteLine();
            Console.WriteLine(ConsoleHelper.CenterText("--- Due Today ---", width));
            Console.WriteLine(new string('-', width));

            foreach (var p in dueToday)
            {
                Console.WriteLine(ConsoleHelper.CenterText($"{p.PlantId} | {p.Name} | {p.Location}", width));
            }

            Console.WriteLine();
            Console.WriteLine(ConsoleHelper.CenterText("--- Upcoming ---", width));
            Console.WriteLine(new string('-', width));

            foreach (var p in upcoming)
            {
                Console.WriteLine(ConsoleHelper.CenterText($"{p.PlantId} | {p.Name} | {p.Location}", width));
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

        public double GetAverageWateringInterval()
        /*
        Calculates the average watering interval across all plants.

        This method computes the mean value of the WateringDays property
        for all plants in the system.

        Returns 0 if no plants exist.
        */
        {
            if (!plants.Any())
                return 0;

            return plants.Average(p => p.WateringDays);
        }
    }
}