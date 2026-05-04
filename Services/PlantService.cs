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

        public void AddPlant(Plant plant)
        {
            plant.PlantId = GeneratePlantId();
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