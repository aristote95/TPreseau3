using Microsoft.EntityFrameworkCore;

namespace TPreseau3.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Définition des DbSet pour les entités
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SecuritySetting> SecuritySettings { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SecuritySetting>().HasNoKey();
            // Configuration des relations et contraintes
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId);

             modelBuilder.Entity<Log>()
               .HasOne(l => l.User)
               .WithMany() // Si un utilisateur peut avoir plusieurs logs
               .HasForeignKey(l => l.UserId);
            // Configurations spécifiques des entités si nécessaire
            // Exemple :
            // modelBuilder.Entity<Role>().HasKey(r => r.Id);
        }
    }
}
