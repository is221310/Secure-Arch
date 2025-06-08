namespace SecurityArch.Models;

    public class Customer
    {
        public int id { get; set; }
        public string vorname { get; set; }
        public string nachname { get; set; }
        public string email { get; set; }
        public string? telefon { get; set; }
        public string? adresse { get; set; }
        public DateTime erstellt_am { get; set; }
    }