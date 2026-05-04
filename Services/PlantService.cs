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
                Type = type
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
        // GetPlantsByType()

        public List<Plant> GetPlantsByType(string type)
        /*
        Returns all plants matching a specific type.
        */
        {
            return plants
                .Where(p => p.Type != null &&
                            p.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        //*****************************************************************************
        // CountByType()

        public Dictionary<string, int> CountByType()
        /*
        Counts how many plants exist for each type.
        */
        {
            return plants
                .GroupBy(p => p.Type)
                .ToDictionary(g => g.Key, g => g.Count());
        }
        //*****************************************************************************


    }
}