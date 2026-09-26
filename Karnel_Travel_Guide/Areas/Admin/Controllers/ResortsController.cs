
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ResortsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ResortsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: RESORTS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Resorts.ToListAsync());
        }

        // GET: RESORTS/Details/5
        public async Task<IActionResult> Details(int? resortid)
        {
            if (resortid == null)
            {
                return NotFound();
            }

            var resort = await _context.Resorts
                .FirstOrDefaultAsync(m => m.ResortID == resortid);
            if (resort == null)
            {
                return NotFound();
            }

            return View(resort);
        }

        // GET: RESORTS/Create
        public IActionResult Create()
        {
            return View();
        }

      
        // POST: /Admin/Resorts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resort resort, List<IFormFile> imageFiles)
        {
            // Model validation error se bachne ke liye ImageUrl clear karein
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                List<string> imagePaths = new List<string>();

                // Multiple images save to wwwroot/images/resorts
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "resorts");
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

                            imagePaths.Add("/images/resorts/" + uniqueFileName);
                        }
                    }
                }

                // Comma-separated string mein path save karein
                resort.ImageUrl = string.Join(",", imagePaths);

                _context.Add(resort);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Resort created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(resort);
        }

        // GET: RESORTS/Edit/5
        public async Task<IActionResult> Edit(int? resortid)
        {
            if (resortid == null)
            {
                return NotFound();
            }

            var resort = await _context.Resorts.FindAsync(resortid);
            if (resort == null)
            {
                return NotFound();
            }
            return View(resort);
        }

    
        [HttpPost]
[Route("Admin/Resorts/Edit/{id?}")] // Route attribute add karein
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Resort resort, List<IFormFile>? imageFiles, List<string>? imagesToDelete)
        {
            if (id != resort.ResortID) return NotFound();

            if (ModelState.IsValid)
            {
                // 1. Existing images list nikaalein
                var currentImages = !string.IsNullOrEmpty(resort.ImageUrl)
                    ? resort.ImageUrl.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    : new List<string>();

                // 2. Selected images remove karein
                if (imagesToDelete != null && imagesToDelete.Count > 0)
                {
                    foreach (var imgPath in imagesToDelete)
                    {
                        currentImages.Remove(imgPath);

                        // Physical File remove karna
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imgPath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                // 3. New images upload aur append karein
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

                // 4. Update model string
                resort.ImageUrl = string.Join(",", currentImages);

                _context.Update(resort);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resort);
        }
        // GET: RESORTS/Delete/5
        public async Task<IActionResult> Delete(int? resortid)
        {
            if (resortid == null)
            {
                return NotFound();
            }

            var resort = await _context.Resorts
                .FirstOrDefaultAsync(m => m.ResortID == resortid);
            if (resort == null)
            {
                return NotFound();
            }

            return View(resort);
        }

        // POST: RESORTS/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int? resortid)
        //{
        //    var resort = await _context.Resorts.FindAsync(resortid);
        //    if (resort != null)
        //    {
        //        // Delete image file from folder upon deletion
        //        if (!string.IsNullOrEmpty(resort.ImageUrl))
        //        {
        //            string imagePath = Path.Combine(
        //                _environment.WebRootPath,
        //                resort.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
        //            );

        //            if (System.IO.File.Exists(imagePath))
        //            {
        //                System.IO.File.Delete(imagePath);
        //            }
        //        }

        //        _context.Resorts.Remove(resort);
        //        await _context.SaveChangesAsync();
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        //private bool ResortExists(int? resortid)
        //{
        //    return _context.Resorts.Any(e => e.ResortID == resortid);
        //}
        // POST: /Admin/Resorts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? resortid)
        {
            var resort = await _context.Resorts.FindAsync(resortid);

            if (resort != null)
            {
                // Multi-image cleanup on delete
                if (!string.IsNullOrEmpty(resort.ImageUrl))
                {
                    var images = resort.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var imgPath in images)
                    {
                        string fullPath = Path.Combine(_environment.WebRootPath, imgPath.Trim().TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                _context.Resorts.Remove(resort);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Resort deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ResortExists(int id)
        {
            return _context.Resorts.Any(e => e.ResortID == id);
        }
    }
}
    
