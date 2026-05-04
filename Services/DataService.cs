// DataService.cs

using PlantCareTracker.Models;
using System.Text.Json;

namespace PlantCareTracker.Services
{
    public class DataService
    {
        public (List<Plant>, List<WateringRecord>) LoadData(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Data file not found. Starting empty.");
                    return (new List<Plant>(), new List<WateringRecord>());
                }

                string json = File.ReadAllText(path);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<PlantData>(json, options);

                return (
                    data?.Plants ?? new List<Plant>(),
                    data?.WateringRecords ?? new List<WateringRecord>()
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data file: {ex.Message}");
                return (new List<Plant>(), new List<WateringRecord>());
            }
        }
    }
}