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

        // GET:Status Pending
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Pending()
        {
            var pendingList = await _context.Appointments
                .Where(a => a.Status == "Pending")
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .Include(a => a.User)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.StartTime)
                .ToListAsync();

            return View(pendingList);
        }

        // POST: Status Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = "Approved";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
        }

        // POST: Status Reject
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = "Rejected";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Pending));
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
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create([Bind("Date,StartTime,EndTime,ServiceId,TrainerId,GymId")] Appointment appointment)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Challenge(); // login sayfasına atar

            appointment.CreatedAt = DateTime.Now; //DateTime.UtcNow
            appointment.UserId = userId;

            appointment.Status = "Pending";

            //Price Service'ten otomatik gelecek
            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == appointment.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("ServiceId", "Geçersiz service seçildi.");
            }
            else
            {
                appointment.Price = service.Price;
            }
            //Saat Kontrolu
            if (appointment.EndTime <= appointment.StartTime)
                ModelState.AddModelError("", "Bitiş saati başlangıçtan büyük olmalı.");

            //Trainer Musaitlik Kontrolu
            var dayOfWeek = appointment.Date.DayOfWeek;

            bool isAvailable = await _context.TrainerAvailabilities.AnyAsync(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Day == dayOfWeek &&
                a.StartTime <= appointment.StartTime &&
                a.EndTime >= appointment.EndTime
            );
            if (!isAvailable)
            {
                ModelState.AddModelError("", "Seçilen trainer bu gün ve saatlerde müsait değil.");
            }

            //Trainer Çakışma Kontrolu
            bool hasConflict = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Date.Date == appointment.Date.Date &&
                a.StartTime < appointment.EndTime &&
                a.EndTime > appointment.StartTime &&
                a.Status != "Rejected"
            );
            if (hasConflict)
            {
                ModelState.AddModelError("", "Bu saatlerde trainer'ın başka bir randevusu var.");
            }

            //Gym Çalışma Saatleri Kontrolu
            var gym = await _context.Gyms.FindAsync(appointment.GymId);
            if (gym != null)
            {
                if (appointment.StartTime < gym.OpenTime || appointment.EndTime > gym.CloseTime)
                {
                    ModelState.AddModelError("", "Randevu gym çalışma saatleri dışında.");
                }
            }

            //Trainer'ın Service Verip Vermediği Kontrolü
            bool trainerCanDoService = await _context.TrainerServices.AnyAsync(ts =>
                ts.TrainerId == appointment.TrainerId &&
                ts.ServiceId == appointment.ServiceId
            );

            if (!trainerCanDoService)
                ModelState.AddModelError("", "Seçilen trainer bu service'i vermiyor.");



            if (ModelState.IsValid)
            {

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(MyAppointments));

            }
            ViewData["GymId"] = new SelectList(_context.Gyms, "GymId", "Name", appointment.GymId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", appointment.ServiceId);
            //ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", appointment.TrainerId);
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", appointment.UserId);
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
                if (User.IsInRole("Admin"))
                    return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(MyAppointments));
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
            if (User.IsInRole("Admin"))
                return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(MyAppointments));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
