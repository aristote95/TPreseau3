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

        // R�cup�rer tous les r�les
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _context.Roles.ToList();
            return Ok(roles);
        }

        // Ajouter un nouveau r�le
        [HttpPost]
        public IActionResult CreateRole(Role role)
        {
            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("Le nom du r�le est requis.");
            }

            _context.Roles.Add(role);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetRoles), new { id = role.RoleId }, role);
        }

        // Mettre � jour un r�le existant
        [HttpPut("{id}")]
        public IActionResult UpdateRole(int id, Role updatedRole)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound("R�le non trouv�.");
            }

            role.RoleName = updatedRole.RoleName;
            _context.SaveChanges();
            return Ok(role);
        }

        // Supprimer un r�le
        [HttpDelete("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null)
            {
                return NotFound("R�le non trouv�.");
            }

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
