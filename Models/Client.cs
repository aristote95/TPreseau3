namespace TPreseau3.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientType { get; set; } // "residential" or "business"
    }
}