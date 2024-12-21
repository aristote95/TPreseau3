namespace TPreseau3.Models
{
    public class Log
    {
        public int LogId { get; set; } // Identifiant unique pour chaque log
        public int UserId { get; set; } // Utilisateur associ� � l'action
        public string EventType { get; set; } = string.Empty; // Type d'�v�nement (ex. : "Login", "PasswordChange")
        public DateTime EventDate { get; set; } = DateTime.UtcNow; // Date et heure de l'�v�nement
        public string Details { get; set; } = string.Empty; // D�tails suppl�mentaires sur l'�v�nement
        
        // Relation avec la table User
        public User User { get; set; } = null!;
    }
}
