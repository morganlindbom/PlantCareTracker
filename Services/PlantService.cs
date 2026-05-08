// PlantService.cs

using PlantCareTracker.Models;

namespace PlantCareTracker.Services
{
    public class PlantService
    {
        private readonly List<Plant> plants;
        private int plantCounter = 1;
        //*****************************************************************************
        public PlantService(List<Plant> plants)
        {
            this.plants = plants ?? new List<Plant>();
            InitializeCounter();
        }
        //*****************************************************************************
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
        //*****************************************************************************
        private string GeneratePlantId()
        {
            return $"PLANT-{plantCounter++.ToString("D3")}";
        }
        //*****************************************************************************
        public List<Plant> GetAllPlants()
        {
            return plants;
        }
        //*****************************************************************************
        // AddPlant()

        public void AddPlant(string name, string location, int wateringDays, string type)
        {
            var plant = new Plant
            {
                PlantId = GeneratePlantId(),
                Name = name,
                Location = location,
                WateringDays = wateringDays,
                Type = type,
                HealthStatus = HealthStatus.Healthy
            };

            plants.Add(plant);
        }
        //*****************************************************************************
        public bool DeletePlant(string id)
        {
            var plant = plants.FirstOrDefault(p => p.PlantId == id);

            if (plant == null)
                return false;

            plants.Remove(plant);
            return true;
        }
        //*****************************************************************************
        public List<Plant> SearchPlant(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new List<Plant>();

            return plants
                .Where(p => !string.IsNullOrWhiteSpace(p.Name) &&
                            p.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        //*****************************************************************************
        public List<Plant> GetPlantsByType(string type)
        /*
        Returns all plants matching a given type.

        Uses case-insensitive comparison to improve usability.
        */
        {
            return plants
                .Where(p => p.Type != null &&
                            p.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        //*****************************************************************************
        // UpdateHealth()

        public bool UpdateHealth(string plantId, HealthStatus status)
        /*
        Updates the health status of a plant by ID.
        */
        {
            var plant = plants.FirstOrDefault(p => p.PlantId == plantId);

            if (plant == null)
                return false;

            plant.HealthStatus = status;
            return true;
        }
        //*****************************************************************************
        // GetStrugglingPlants()

        public List<Plant> GetStrugglingPlants()
        /*
        Returns all plants with health status "Struggling".
        */
        {
            return plants
                .Where(p => p.HealthStatus == HealthStatus.Struggling)
                .ToList();
        }
        //*****************************************************************************
        // CountPlantsByType

        public Dictionary<string, int> CountPlantsByType()
        /*
        Counts how many plants exist for each type.

        Groups plants by their Type property and returns
        a dictionary where:
        - Key = Type
        - Value = Number of plants
        */
        {
            return plants
                .Where(p => !string.IsNullOrWhiteSpace(p.Type))
                .GroupBy(p => p.Type)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        //*****************************************************************************

    }
}