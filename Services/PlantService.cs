using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using PlantCareTracker.Models;

namespace PlantCareTracker.Services
{
    public class PlantService
    {

        private List<Plant> plants = new List<Plant>();
        private List<WateringRecord> records = new List<WateringRecord>();

        public void AddPlant(Plant plant)
        {
            plants.Add(plant);
        }

        public List<Plant> GetAllPlants()
        {
            return plants;
        }

        public bool DeletePlant(string id)
        {
            var plant = plants.FirstOrDefault(p => p.PlantId == id);

            if (plant == null)
            {
                return false;
            }

            plants.Remove(plant);
            return true;
        }

        public List<Plant> SearchPlant(string name)
        {
            return plants
                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                .ToList();

        }

        public void LogWatering(string plantId, string notes)
        {
            records.Add(new WateringRecord
            {
                PlantId = plantId,
                Date = DateTime.Now,
                Notes = notes
            });
        }


        public List<WateringRecord> GetAllLogs()
        {
            return records;
        }

        public List<WateringRecord> GetLogsForPlant(string plantId)
        {
            return records
                .Where(r => r.PlantId == plantId)
                .ToList();
        }

        public void ShowReminders()
        {

            foreach (var plant in plants)
            {
                var last = records
                    .Where(r => r.PlantId == plant.PlantId)
                    .OrderByDescending(records => records.Date)
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