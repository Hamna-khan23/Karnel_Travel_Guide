using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public HotelsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: /Admin/Hotels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hotels.ToListAsync());
        }

        // GET: /Admin/Hotels/Details/5
        public async Task<IActionResult> Details(int? hotelid)
        {
            if (hotelid == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelID == hotelid);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // GET: /Admin/Hotels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Hotels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Hotel hotel, List<IFormFile> imageFiles)
        {
            // Model state check hone se pehle ImageUrl property error clear karein agar validation error aaye
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                List<string> imagePaths = new List<string>();

                // Multiple images handling & saving to wwwroot/images/hotels
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "hotels");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            imagePaths.Add("/images/hotels/" + uniqueFileName);
                        }
                    }
                }

                // Comma separated string me save karein (Edit action format se match rakhne ke liye)
                hotel.ImageUrl = string.Join(",", imagePaths);

                _context.Add(hotel);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hotel created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(hotel);
        }

        // GET: /Admin/Hotels/Edit/5
        public async Task<IActionResult> Edit(int? hotelid)
        {
            if (hotelid == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FindAsync(hotelid);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: Admin/Hotels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int hotelid, Hotel hotel, List<IFormFile> imageFiles, List<string> removeImages)
        {
            if (hotelid != hotel.HotelID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingHotel = await _context.Hotels.AsNoTracking().FirstOrDefaultAsync(h => h.HotelID == hotelid);
                    if (existingHotel == null) return NotFound();

                    List<string> currentImages = new List<string>();
                    if (!string.IsNullOrEmpty(existingHotel.ImageUrl))
                    {
                        currentImages = existingHotel.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                    }

                    // 1. User ne jin images ko remove/select kiya hai unko list aur disk dono se delete karein
                    if (removeImages != null && removeImages.Count > 0)
                    {
                        foreach (var imgToRemove in removeImages)
                        {
                            currentImages.Remove(imgToRemove);

                            // File System se delete
                            string fullPath = Path.Combine(_environment.WebRootPath, imgToRemove.TrimStart('/'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }

                    // 2. Nayi uploaded images ko append karein
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "hotels");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        foreach (var file in imageFiles)
                        {
                            if (file.Length > 0)
                            {
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }
                                currentImages.Add("/images/hotels/" + uniqueFileName);
                            }
                        }
                    }

                    hotel.ImageUrl = string.Join(",", currentImages);

                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hotel updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.HotelID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        // GET: /Admin/Hotels/Delete/5
        public async Task<IActionResult> Delete(int? hotelid)
        {
            if (hotelid == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(m => m.HotelID == hotelid);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        // POST: /Admin/Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? hotelid)
        {
            var hotel = await _context.Hotels.FindAsync(hotelid);

            if (hotel != null)
            {
                // Multi-image comma separated cleanup on delete
                if (!string.IsNullOrEmpty(hotel.ImageUrl))
                {
                    var images = hotel.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var imgPath in images)
                    {
                        string fullPath = Path.Combine(_environment.WebRootPath, imgPath.Trim().TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelID == id);
        }
    }
}