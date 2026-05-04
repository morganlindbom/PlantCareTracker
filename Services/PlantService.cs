// PlantService.cs

using PlantCareTracker.Models;

namespace PlantCareTracker.Services
{
    public class PlantService
    {
        private readonly List<Plant> plants;
        private int plantCounter = 1;

        public PlantService(List<Plant> plants)
        {
            this.plants = plants ?? new List<Plant>();
            InitializeCounter();
        }

        private void InitializeCounter()
        {
            if (!plants.Any())
                return;

            var numbers = plants
                .Select(p => p.PlantId)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id =>
                {
                    var parts = id.Split('-');
                    return int.TryParse(parts.Last(), out int num) ? num : 0;
                })
                .Where(n => n > 0)
                .ToList();

            if (numbers.Any())
            {
                plantCounter = numbers.Max() + 1;
            }
        }

        private string GeneratePlantId()
        {
            return $"PLANT-{plantCounter++.ToString("D3")}";
        }

        public List<Plant> GetAllPlants()
        {
            return plants;
        }

        // AddPlant()

        public void AddPlant(string name, string location, int wateringDays)
        /*
        Creates and adds a new plant.

        This method is responsible for creating the Plant object,
        assigning an ID, and storing it in the list.
        */
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            if (wateringDays < 1)
                throw new ArgumentException("Watering days must be at least 1.");

            var plant = new Plant
            {
                PlantId = GeneratePlantId(),
                Name = name,
                Location = location,
                WateringDays = wateringDays
            };

            plants.Add(plant);
        }

        public bool DeletePlant(string id)
        {
            var plant = plants.FirstOrDefault(p => p.PlantId == id);

            if (plant == null)
                return false;

            plants.Remove(plant);
            return true;
        }

        public List<Plant> SearchPlant(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Plant>();

            return plants
                .Where(p => !string.IsNullOrWhiteSpace(p.Name) &&
                            p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}