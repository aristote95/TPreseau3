using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TPreseau3.Models;

namespace TPreseau3.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("residential")]
        [Authorize(Roles = "Administrateur,Préposé aux clients résidentiels")]
        public IActionResult GetResidentialClients()
        {
            var clients = _context.Clients
                .Where(c => c.ClientType == "residential")
                .ToList();
            return Ok(clients);
        }

        [HttpGet("business")]
        [Authorize(Roles = "Administrateur,Préposé aux clients d'affaires")]
        public IActionResult GetBusinessClients()
        {
            var clients = _context.Clients
                .Where(c => c.ClientType == "business")
                .ToList();
            return Ok(clients);
        }
    }


}


