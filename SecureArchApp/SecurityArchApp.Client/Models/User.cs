namespace SecureArchApp.Client.Models
{
    public class User
    {
        public int id { get; set; }
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string? password { get; set; }
        public string role { get; set; } = string.Empty;
        public string? telephone { get; set; }
        public string? address { get; set; }
        public int? kunden_id { get; set; }

        public string? kunden_name { get; set; }
    }
}
