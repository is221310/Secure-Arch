using System.Text.Json.Serialization;

namespace SecureArchCore.Models
{
    public class Sensor
    {
        public int sensor_id { get; set; }
        public string sensor_name { get; set; }
        public string beschreibung { get; set; }
        public DateTime created_at { get; set; }

        public int? kunden_id { get; set; }
        [JsonIgnore]
        public Kunde? Kunde { get; set; }

        public List<string> ip_addresses { get; set; } = new();
        public List<Temperatur> Temperaturen { get; set; } = new();
        public ICollection<IpResult> IpResults { get; set; } = new List<IpResult>();

        public string? secret_key { get; set; }
    }
}
