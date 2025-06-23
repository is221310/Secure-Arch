using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecureArchCore.Models
{
    public class Temperatur
    {
        [Key]
        public int id { get; set; }


        public int sensor_id { get; set; }

        public double temperatur { get; set; }

        public DateTime timestamp { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        [ForeignKey("sensor_id")]
        public virtual Sensor Sensor { get; set; } = null!;
    }
}
