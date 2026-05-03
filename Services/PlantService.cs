using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
