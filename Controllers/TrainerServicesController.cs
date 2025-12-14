using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenterProject.Data;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Controllers
{
    public class TrainerServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainerServices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TrainerServices.Include(t => t.Service).Include(t => t.Trainer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TrainerServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerService = await _context.TrainerServices
                .Include(t => t.Service)
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.TrainerServiceId == id);
            if (trainerService == null)
            {
                return NotFound();
            }

            return View(trainerService);
        }

        // GET: TrainerServices/Create
        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name");
            return View();
        }

        // POST: TrainerServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainerServiceId,TrainerId,ServiceId")] TrainerService trainerService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainerService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", trainerService.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", trainerService.TrainerId);
            return View(trainerService);
        }

        // GET: TrainerServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerService = await _context.TrainerServices.FindAsync(id);
            if (trainerService == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", trainerService.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", trainerService.TrainerId);
            return View(trainerService);
        }

        // POST: TrainerServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerServiceId,TrainerId,ServiceId")] TrainerService trainerService)
        {
            if (id != trainerService.TrainerServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainerService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerServiceExists(trainerService.TrainerServiceId))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "Name", trainerService.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "TrainerId", "Name", trainerService.TrainerId);
            return View(trainerService);
        }

        // GET: TrainerServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerService = await _context.TrainerServices
                .Include(t => t.Service)
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.TrainerServiceId == id);
            if (trainerService == null)
            {
                return NotFound();
            }

            return View(trainerService);
        }

        // POST: TrainerServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainerService = await _context.TrainerServices.FindAsync(id);
            if (trainerService != null)
            {
                _context.TrainerServices.Remove(trainerService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerServiceExists(int id)
        {
            return _context.TrainerServices.Any(e => e.TrainerServiceId == id);
        }
    }
}
