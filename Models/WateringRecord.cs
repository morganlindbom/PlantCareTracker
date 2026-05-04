// WateringRecord.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantCareTracker.Models
{
    public class WateringRecord
    {
        public string PlantId { get; set; } = string.Empty;
        public DateTime Date { get; set; } 
        public string Notes { get; set; } = string.Empty;
    }
}
