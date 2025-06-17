using System.Text.Json.Serialization;

namespace SecureArchApp.Client.Models
{
    public class Sensor
    {
        [JsonPropertyName("sensor_id")]
        public int sensor_id { get; set; }

        [JsonPropertyName("sensor_name")]
        public string sensor_name { get; set; } = "";

        [JsonPropertyName("beschreibung")]
        public string beschreibung { get; set; } = "";

        [JsonPropertyName("kunden_id")]
        public int? kunden_id { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
