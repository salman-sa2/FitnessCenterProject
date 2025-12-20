using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FitnessCenterProject.Data;

namespace FitnessCenterProject.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /api/AppointmentsApi/member
        // Current user'ın randevularını getir
        [HttpGet("member")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetMemberAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.StartTime)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.Date,
                    a.StartTime,
                    a.EndTime,
                    a.Status,
                    a.Price,
                    a.CreatedAt,
                    Service = new
                    {
                        a.Service!.ServiceId,
                        a.Service.Name,
                        a.Service.Duration
                    },
                    Trainer = new
                    {
                        a.Trainer!.TrainerId,
                        a.Trainer.Name,
                        a.Trainer.Specialization
                    },
                    Gym = new
                    {
                        a.Gym!.GymId,
                        a.Gym.Name
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: /api/AppointmentsApi/member/{userId}
        // Belirli bir üyenin randevularını getir (Admin için)
        // Kullanım: /api/AppointmentsApi/member/{userId} - userId'yi veritabanından veya başka bir endpoint'ten alabilirsiniz
        [HttpGet("member/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMemberAppointmentsById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required");
            }

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.StartTime)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.Date,
                    a.StartTime,
                    a.EndTime,
                    a.Status,
                    a.Price,
                    a.CreatedAt,
                    Service = new
                    {
                        a.Service!.ServiceId,
                        a.Service.Name,
                        a.Service.Duration
                    },
                    Trainer = new
                    {
                        a.Trainer!.TrainerId,
                        a.Trainer.Name,
                        a.Trainer.Specialization
                    },
                    Gym = new
                    {
                        a.Gym!.GymId,
                        a.Gym.Name
                    },
                    User = new
                    {
                        a.User!.Id,
                        a.User.Email
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: /api/AppointmentsApi/member/email/{email}
        // Email ile üye randevularını getir (Admin için - daha kullanışlı)
        [HttpGet("member/email/{email}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMemberAppointmentsByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"User with email {email} not found");
            }

            var appointments = await _context.Appointments
                .Where(a => a.UserId == user.Id)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.StartTime)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.Date,
                    a.StartTime,
                    a.EndTime,
                    a.Status,
                    a.Price,
                    a.CreatedAt,
                    Service = new
                    {
                        a.Service!.ServiceId,
                        a.Service.Name,
                        a.Service.Duration
                    },
                    Trainer = new
                    {
                        a.Trainer!.TrainerId,
                        a.Trainer.Name,
                        a.Trainer.Specialization
                    },
                    Gym = new
                    {
                        a.Gym!.GymId,
                        a.Gym.Name
                    },
                    User = new
                    {
                        a.User!.Id,
                        a.User.Email
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: /api/AppointmentsApi/all
        // Tüm randevuları getir (Admin için)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .Include(a => a.User)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.StartTime)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.Date,
                    a.StartTime,
                    a.EndTime,
                    a.Status,
                    a.Price,
                    a.CreatedAt,
                    Service = new
                    {
                        a.Service!.ServiceId,
                        a.Service.Name,
                        a.Service.Duration
                    },
                    Trainer = new
                    {
                        a.Trainer!.TrainerId,
                        a.Trainer.Name,
                        a.Trainer.Specialization
                    },
                    Gym = new
                    {
                        a.Gym!.GymId,
                        a.Gym.Name
                    },
                    User = new
                    {
                        a.User!.Id,
                        a.User.Email
                    }
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // GET: /api/AppointmentsApi/{id}
        // Belirli bir randevuyu getir
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .Include(a => a.User)
                .Where(a => a.AppointmentId == id && (isAdmin || a.UserId == userId))
                .Select(a => new
                {
                    a.AppointmentId,
                    a.Date,
                    a.StartTime,
                    a.EndTime,
                    a.Status,
                    a.Price,
                    a.CreatedAt,
                    Service = new
                    {
                        a.Service!.ServiceId,
                        a.Service.Name,
                        a.Service.Duration
                    },
                    Trainer = new
                    {
                        a.Trainer!.TrainerId,
                        a.Trainer.Name,
                        a.Trainer.Specialization
                    },
                    Gym = new
                    {
                        a.Gym!.GymId,
                        a.Gym.Name
                    },
                    User = isAdmin ? new
                    {
                        a.User!.Id,
                        a.User.Email
                    } : null
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }
    }
}
