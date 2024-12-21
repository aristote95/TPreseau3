using Microsoft.AspNetCore.Mvc;
using TPreseau3.Models;

namespace TPreseau3.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les rôles
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _context.Roles.ToList();
            return Ok(roles);
        }

        // Ajouter un nouveau rôle
        [HttpPost]
        public IActionResult CreateRole(Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("Le nom du rôle est requis.");
            }

            _context.Roles.Add(role);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetRoles), new { id = role.RoleId }, role);
        }

        // Mettre à jour un rôle existant
        [HttpPut("{id}")]
        public IActionResult UpdateRole(int id, Role updatedRole)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound("Rôle non trouvé.");
            }

            role.RoleName = updatedRole.RoleName;
            _context.SaveChanges();
            return Ok(role);
        }

        // Supprimer un rôle
        [HttpDelete("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound("Rôle non trouvé.");
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
