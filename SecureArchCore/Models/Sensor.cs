using SecurityArch.Models;
using System.Text.Json.Serialization;

namespace SecureArchCore.Models
{
    public class Sensor
    {
        public int id { get; set; }
        public string ip { get; set; }
        public string beschreibung { get; set; }

        public int? kunden_id { get; set; }
        [JsonIgnore]
        public Customer customer { get; set; }
    }
}
