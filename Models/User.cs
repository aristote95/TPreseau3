namespace TPreseau3.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
        
        public int FailedAttempts { get; set; } = 0; // Nombre de tentatives échouées
        public bool IsLocked { get; set; } = false; // Statut du verrouillage


        public Role Role { get; set; } = null!; // Relation avec la table Roles
    }
}
