namespace TPreseau3.Models
{
    public class Log
    {
        public int LogId { get; set; } // Identifiant unique pour chaque log
        public int UserId { get; set; } // Utilisateur associé à l'action
        public string EventType { get; set; } = string.Empty; // Type d'événement (ex. : "Login", "PasswordChange")
        public DateTime EventDate { get; set; } = DateTime.UtcNow; // Date et heure de l'événement
        public string Details { get; set; } = string.Empty; // Détails supplémentaires sur l'événement
        
        // Relation avec la table User
        public User User { get; set; } = null!;
    }
}
