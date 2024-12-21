using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPreseau3.Models;
using BCrypt.Net;

namespace TPreseau3.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les utilisateurs
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Email,
                    Role = u.Role.RoleName
                })
                .ToList();

            return Ok(users);
        }

        // Ajouter un utilisateur avec hachage de mot de passe
        [HttpPost]
        [Authorize(Roles = "Administrateur")]
        public IActionResult CreateUser(User user)
        {
            // Vérification des champs obligatoires
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Le nom d'utilisateur et le mot de passe sont requis.");
            }

            // Vérification de la complexité du mot de passe
            if (!IsPasswordComplex(user.Password))
            {
                return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une lettre majuscule, une lettre minuscule, un chiffre et un caractère spécial.");
            }

            // Vérification du rôle
            var role = _context.Roles.Find(user.RoleId);
            if (role == null)
            {
                return BadRequest("Le rôle spécifié n'existe pas.");
            }

            // Hachage du mot de passe
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Ajouter l'utilisateur
            _context.Users.Add(user);
            _context.SaveChanges();

            // Retourner l'utilisateur créé
            return CreatedAtAction(nameof(GetUsers), new { id = user.UserId }, new
            {
                user.UserId,
                user.Username,
                user.Email,
                Role = role.RoleName
            });
        }

        // Vérification de la complexité du mot de passe
        private bool IsPasswordComplex(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => "!@#$%^&*()".Contains(c));
        }

        // Mettre à jour un utilisateur
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User updatedUser)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Mise à jour des champs
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                if (!IsPasswordComplex(updatedUser.Password))
                {
                    return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une lettre majuscule, une lettre minuscule, un chiffre et un caractère spécial.");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            }

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.RoleId = updatedUser.RoleId;
            _context.SaveChanges();

            return Ok(new
            {
                user.UserId,
                user.Username,
                user.Email,
                Role = _context.Roles.Find(user.RoleId)?.RoleName
            });
        }

        // Supprimer un utilisateur
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
