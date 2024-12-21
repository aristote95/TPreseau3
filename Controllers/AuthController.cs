using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TPreseau3.Models;
using Microsoft.EntityFrameworkCore;

namespace TPreseau3.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Recherche de l'utilisateur dans la base de données
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);

            if (user == null)
            {
                // Journaliser une tentative échouée pour un utilisateur inexistant
                LogFailedLogin(null, $"Tentative échouée pour {request.Username}: utilisateur non trouvé.");
                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
            }

            // Vérifier si le compte est verrouillé
            if (user.IsLocked)
            {
                return Unauthorized("Compte verrouillé. Contactez l'administrateur.");
            }

            // Vérifier le mot de passe avec BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                user.FailedAttempts++;

                // Verrouiller le compte après trop de tentatives échouées
                if (user.FailedAttempts >= 5)
                {
                    user.IsLocked = true;
                    LogAccountLocked(user);
                }

                LogFailedLogin(user, $"Nombre d'échecs : {user.FailedAttempts}");
                _context.SaveChanges();

                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
            }

            // Réinitialiser les tentatives échouées
            user.FailedAttempts = 0;

            // Journaliser une connexion réussie
            LogSuccessfulLogin(user);

            // Sauvegarder les changements dans la base de données
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Erreur lors de l'enregistrement des logs : {ex.InnerException?.Message}");
            }

            // Générer un jeton JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("YourSecretKeyHere");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.RoleName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                Message = "Connexion réussie",
                Token = tokenHandler.WriteToken(token),
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role.RoleName
            });
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { message = "Le nom d'utilisateur et le mot de passe sont requis." });
                }

                var role = _context.Roles.FirstOrDefault(r => r.RoleName == request.Role);
                if (role == null)
                {
                    return BadRequest(new { message = "Le rôle spécifié est invalide." });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    RoleId = role.RoleId
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { message = "Compte créé avec succès." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'enregistrement : {ex.Message}");
                return StatusCode(500, new { message = "Une erreur est survenue. Veuillez réessayer." });
            }
        }

        private bool IsPasswordComplex(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => "!@#$%^&*()".Contains(c));
        }

        private void LogFailedLogin(User? user, string details)
        {
            _context.Logs.Add(new Log
            {
                UserId = user?.UserId ?? 0,
                EventType = "LoginFailed",
                EventDate = DateTime.UtcNow,
                Details = details
            });
        }

        private void LogAccountLocked(User user)
        {
            _context.Logs.Add(new Log
            {
                UserId = user.UserId,
                EventType = "AccountLocked",
                EventDate = DateTime.UtcNow,
                Details = $"{user.Username} a été verrouillé après trop de tentatives échouées."
            });
        }

        private void LogSuccessfulLogin(User user)
        {
            _context.Logs.Add(new Log
            {
                UserId = user.UserId,
                EventType = "LoginSuccess",
                EventDate = DateTime.UtcNow,
                Details = $"{user.Username} s'est connecté avec succès."
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Rôle par défaut
    }
}
