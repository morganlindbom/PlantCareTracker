// DataService.cs

using PlantCareTracker.Models;
using System.Text.Json;

namespace PlantCareTracker.Services
{
    public class DataService
    {
        public (List<Plant>, List<WateringRecord>) LoadData(string path)
        /*
        Loads plant and watering data from a JSON file.

        Reads the JSON file, deserializes plants and watering records,
        and returns both lists.

        If the file is missing or invalid, empty lists are returned.
        */
        {
            List<Plant> plants = new();
            List<WateringRecord> records = new();

            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var data = JsonSerializer.Deserialize<JsonElement>(json);

                    plants = JsonSerializer.Deserialize<List<Plant>>(
                        data.GetProperty("plants").GetRawText(),
                        options
                    ) ?? new List<Plant>();

                    records = JsonSerializer.Deserialize<List<WateringRecord>>(
                        data.GetProperty("wateringRecords").GetRawText(),
                        options
                    ) ?? new List<WateringRecord>();
                }
                catch
                {
                    Console.WriteLine("Error reading data file. Starting with empty lists.");
                }
            }
            else
            {
                Console.WriteLine("Data file not found. Starting with empty lists.");
            }

            return (plants, records);
        }
    }
}