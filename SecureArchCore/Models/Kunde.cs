namespace SecureArchCore.Models
{
    public class Kunde
    {
        public int kunden_id { get; set; }
        public string kunden_name { get; set; }
        public DateTime created_at { get; set; }

        public ICollection<User>? Users { get; set; }
        public ICollection<Sensor>? Sensoren { get; set; }
    }
}
