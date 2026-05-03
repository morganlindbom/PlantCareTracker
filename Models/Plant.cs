using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantCareTracker.Models
{
    public class Plant
    {
        public string PlantId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int WateringDays { get; set; }
    }
}
