using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Net;

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

    public List<string> ip_addresses { get; set; } = new();
    public List<Temperatur> Temperaturen { get; set; } = new();
    public ICollection<IpResult> IpResults { get; set; } = new List<IpResult>();
}

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