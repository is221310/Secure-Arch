using System.Text.Json.Serialization;

namespace SecureArchCore.Models
{
    public class IpResult
    {
        public int id { get; set; }

        public int sensor_id { get; set; }
        public string ip_address { get; set; } = string.Empty; // als string speichern

        public bool status { get; set; }
        public DateTime timestamp { get; set; }
        [JsonIgnore]
        public Sensor Sensor { get; set; } = null!;
    }
}
