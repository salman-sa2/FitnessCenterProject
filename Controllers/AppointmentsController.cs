using FitnessCenterProject.Data;
using FitnessCenterProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessCenterProject.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Appointments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments.Include(a => a.Gym).Include(a => a.Service).Include(a => a.Trainer).Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myList = await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.Gym)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(myList);
        }


    // GET:Details
    public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET:Create
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST:Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,Date,StartTime,EndTime,Status,Price,ServiceId,TrainerId,GymId")] Appointment appointment)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Challenge(); // login sayfasına atar

            appointment.CreatedAt = DateTime.Now; //DateTime.UtcNow
            appointment.UserId = _userManager.GetUserId(User);
            appointment.Status = "Approved";
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();               
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", appointment.GymId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", appointment.TrainerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // GET:Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", appointment.GymId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", appointment.TrainerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // POST:Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,Date,StartTime,EndTime,Status,Price,CreatedAt,UserId,ServiceId,TrainerId,GymId")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", appointment.GymId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", appointment.TrainerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", appointment.UserId);
            return View(appointment);
        }

        // GET:Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST:Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
