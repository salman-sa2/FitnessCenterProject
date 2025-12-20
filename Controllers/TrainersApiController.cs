using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;

namespace FitnessCenterProject.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public TrainersApiController(ApplicationDbContext context) => _context = context;

        // GET: /api/trainers
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trainers = await _context.Trainers
                .Select(t => new {
                    t.TrainerId,
                    t.Name,
                    t.Email,
                    t.Specialization,
                    t.GymId 
                })
                .ToListAsync();

            return Ok(trainers);
        }

        // GET: /api/trainers/available?date=2025-12-15&start=10:00&end=11:00&gymId=1
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable(
            DateTime date,
            TimeSpan start,
            TimeSpan end,
            int? gymId,
            int? serviceId)
        {
            var day = date.DayOfWeek; ;

            var query = _context.Trainers
                .Include(t => t.Availabilities)
                .Include(t => t.Appointments)
                .Include(t => t.TrainerServices)
                .AsQueryable();

            if (gymId.HasValue)
                query = query.Where(t => t.GymId == gymId.Value);
            if (serviceId.HasValue)
                query = query
                    .Where(t =>
                    t.TrainerServices.Any(ts =>
                    ts.ServiceId == serviceId.Value));

            var available = await query
                .Where(t =>
                    t.Availabilities.Any(a =>
                        a.Day == day &&
                        a.StartTime <= start &&
                        a.EndTime >= end
                    )
                    &&
                    !t.Appointments.Any(ap =>
                        ap.Date.Date == date.Date &&
                        ap.StartTime < end &&
                        ap.EndTime > start &&
                        ap.Status != "Rejected"
                    )
                )
                .Select(t => new { t.TrainerId, t.Name })
                .ToListAsync();

            return Ok(available);
        }
    }
}
