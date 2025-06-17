using System.Text.Json.Serialization;

public class Kunde
{
    public int kunden_id { get; set; }
    public string kunden_name { get; set; }
    public DateTime created_at { get; set; }

    public ICollection<User>? Users { get; set; }
    public ICollection<Sensor>? Sensoren { get; set; }
}

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


public class Sensor
{
    public int sensor_id{ get; set; }
    public string sensor_name { get; set; }
    public string beschreibung { get; set; }
    public DateTime created_at { get; set; }

    public int? kunden_id { get; set; }
    [JsonIgnore]
    public Kunde? Kunde { get; set; }
}