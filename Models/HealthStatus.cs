// HealthStatus.cs

namespace PlantCareTracker.Models
{
    public enum HealthStatus
    /*
    Represents the health condition of a plant.

    Using enum ensures only valid predefined values are allowed.
    */
    {
        Healthy,
        Struggling,
        Thriving
    }
}