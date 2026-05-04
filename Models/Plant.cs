// Plant.cs

namespace PlantCareTracker.Models
{
    public class Plant
    {
        public string PlantId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int WateringDays { get; set; }
        public string Type { get; set; }
        public HealthStatus HealthStatus { get; set; }

        //*********************************************************
        // NeedsWater()

        public bool NeedsWater(DateTime? lastWatered)
        /*
        Determines if the plant needs watering.

        Compares the last watering date with the current date
        and checks if the interval exceeds WateringDays.
        */
        {
            if (lastWatered == null)
                return true;

            int daysSince = (DateTime.Now - lastWatered.Value).Days;

            return daysSince >= WateringDays;
        }
    }
}