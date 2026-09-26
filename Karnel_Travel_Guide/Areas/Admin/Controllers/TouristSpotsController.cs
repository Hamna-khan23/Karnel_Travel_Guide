
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karnel_Travel_Guide.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TouristSpotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TouristSpotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TouristSpots
        public async Task<IActionResult> Index()
        {
            return View(await _context.TouristSpots.ToListAsync());
        }

        // GET: Admin/TouristSpots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var touristspot = await _context.TouristSpots
                .FirstOrDefaultAsync(m => m.SpotID == id);

            if (touristspot == null)
            {
                return NotFound();
            }

            return View(touristspot);
        }

        // GET: Admin/TouristSpots/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TouristSpots/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TouristSpot touristspot, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    List<string> imagePaths = new List<string>();
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            imagePaths.Add("/images/" + fileName);
                        }
                    }
                    touristspot.ImageUrl = string.Join(",", imagePaths);
                }

                _context.Add(touristspot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(touristspot);
        }

        // GET: Admin/TouristSpots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var touristspot = await _context.TouristSpots.FindAsync(id);
            if (touristspot == null)
            {
                return NotFound();
            }
            return View(touristspot);
        }

        // POST: Admin/TouristSpots/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TouristSpot touristspot, List<IFormFile> imageFiles, List<string> removeImages)
        {
            if (id != touristspot.SpotID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSpot = await _context.TouristSpots.AsNoTracking().FirstOrDefaultAsync(t => t.SpotID == id);
                    List<string> currentImages = existingSpot?.ImageUrl?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

                    if (removeImages != null && removeImages.Count > 0)
                    {
                        currentImages.RemoveAll(img => removeImages.Contains(img.Trim()));
                    }

                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        foreach (var file in imageFiles)
                        {
                            if (file.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }
                                currentImages.Add("/images/" + fileName);
                            }
                        }
                    }

                    touristspot.ImageUrl = string.Join(",", currentImages);

                    _context.Update(touristspot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TouristSpotExists(touristspot.SpotID))
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
            return View(touristspot);
        }

        // GET: Admin/TouristSpots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var touristspot = await _context.TouristSpots
                .FirstOrDefaultAsync(m => m.SpotID == id);

            if (touristspot == null)
            {
                return NotFound();
            }

            return View(touristspot);
        }

        // POST: Admin/TouristSpots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var touristspot = await _context.TouristSpots.FindAsync(id);

            if (touristspot != null)
            {
                // Spot se jurray packages ko pehle delete karenge SQL error se bachne ke liye
                var relatedPackages = _context.Packages.Where(p => p.SpotID == id);
                _context.Packages.RemoveRange(relatedPackages);

                _context.TouristSpots.Remove(touristspot);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TouristSpotExists(int id)
        {
            return _context.TouristSpots.Any(e => e.SpotID == id);
        }
    }
}