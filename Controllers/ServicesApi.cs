using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;

namespace FitnessCenterProject.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ServicesApiController(ApplicationDbContext context) => _context = context;

        // GET:
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var s = await _context.Services
                .Where(x => x.ServiceId == id)
                .Select(x => new { x.ServiceId, x.Name, x.Price, x.Duration })
                .FirstOrDefaultAsync();

            if (s == null) return NotFound();
            return Ok(s);
        }

        // GET: ServicesApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var services = await _context.Services
                .Select(s => new { s.ServiceId, s.Name, s.Price, s.Duration })
                .ToListAsync();

            return Ok(services);
        }

    }
}
