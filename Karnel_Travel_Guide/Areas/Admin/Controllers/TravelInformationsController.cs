using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TravelInformationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TravelInformationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var travelInformations = await _context.TravelInformations
                .ToListAsync();

            return View(travelInformations);
        }


        // =========================
        // DETAILS - GET
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelInformation = await _context.TravelInformations
                .FirstOrDefaultAsync(x => x.TravelID == id);

            if (travelInformation == null)
            {
                return NotFound();
            }

            return View(travelInformation);
        }


        // =========================
        // CREATE - GET
        // =========================
        public IActionResult Create()
        {
            return View();
        }


        // =========================
        // CREATE - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TravelID,TransportType,FromCity,ToCity,Route,Price,DepartureTime,ArrivalTime,Availability,Description")]
            TravelInformation travelInformation)
        {
            if (ModelState.IsValid)
            {
                _context.TravelInformations.Add(travelInformation);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(travelInformation);
        }


        // =========================
        // EDIT - GET
        // =========================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelInformation = await _context.TravelInformations
                .FindAsync(id);

            if (travelInformation == null)
            {
                return NotFound();
            }

            return View(travelInformation);
        }


        // =========================
        // EDIT - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TravelID,TransportType,FromCity,ToCity,Route,Price,DepartureTime,ArrivalTime,Availability,Description")]
            TravelInformation travelInformation)
        {
            if (id != travelInformation.TravelID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(travelInformation);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TravelInformationExists(travelInformation.TravelID))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(travelInformation);
        }


        // =========================
        // DELETE - GET
        // =========================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travelInformation = await _context.TravelInformations
                .FirstOrDefaultAsync(x => x.TravelID == id);

            if (travelInformation == null)
            {
                return NotFound();
            }

            return View(travelInformation);
        }


        // =========================
        // DELETE - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var travelInformation = await _context.TravelInformations
                .FindAsync(id);

            if (travelInformation != null)
            {
                _context.TravelInformations.Remove(travelInformation);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // EXISTS
        // =========================
        private bool TravelInformationExists(int id)
        {
            return _context.TravelInformations
                .Any(e => e.TravelID == id);
        }
    }
}