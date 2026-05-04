
// PlantService.cs

using PlantCareTracker.Models;

namespace PlantCareTracker.Services
{
    public class PlantService
    {
        private readonly List<Plant> plants = new();
        private int plantCounter = 1;

        private string GeneratePlantId()
        {
            return $"PLANT-{plantCounter++.ToString("D3")}";
        }

        public void AddPlant(Plant plant)
        {
            plant.PlantId = GeneratePlantId();
            plants.Add(plant);
        }

        public List<Plant> GetAllPlants()
        {
            // IMPORTANT: return SAME list (shared reference)
            return plants;
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

