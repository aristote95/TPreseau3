using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TPreseau3.Models;

namespace TPreseau3.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize(Roles = "Administrateur")] // Accessible uniquement par l'administrateur
    public class LogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogController(AppDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les logs
        [HttpGet]
        public IActionResult GetLogs()
        {
            var logs = _context.Logs.OrderByDescending(l => l.EventDate).ToList();
            return Ok(logs);
        }
    }
}
