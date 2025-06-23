using System.Text.Json.Serialization;

namespace SecureArchCore.Models
{
    public class User
    {
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string telephone { get; set; }
        public string role { get; set; }
        public string address { get; set; }
        public DateTime created { get; set; }

        public int? kunden_id { get; set; }


        [JsonIgnore]
        public Kunde? Kunde { get; set; }
    }
}
