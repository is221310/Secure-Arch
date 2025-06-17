namespace SecureArchApp.Client.Models;

public class Kunde
{
    public int kunden_id { get; set; }
    public string kunden_name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}