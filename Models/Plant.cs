// Plant.cs

namespace PlantCareTracker.Models
{
    public class Plant
    {
        public string PlantId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int WateringDays { get; set; }
    }
}