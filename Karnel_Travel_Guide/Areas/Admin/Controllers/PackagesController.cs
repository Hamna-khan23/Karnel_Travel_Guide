
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PackagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PackagesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // =====================================================
        // INDEX
        // =====================================================
        // GET: /Admin/Packages
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var packages = await _context.Packages
                .Include(p => p.TouristSpot)
                .Include(p => p.Hotel)
                .Include(p => p.Resort)
                .Include(p => p.Restaurant)
                .Include(p => p.TravelInformation)
                .Include(p => p.Bookings)
                .ToListAsync();

            return View(packages);
        }


        // =====================================================
        // DETAILS - GET
        // =====================================================
        // GET: /Admin/Packages/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? packageid)
        {
            if (packageid == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.TouristSpot)
                .Include(p => p.Hotel)
                .Include(p => p.Resort)
                .Include(p => p.Restaurant)
                .Include(p => p.TravelInformation)
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.PackageID == packageid);

            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }
        // GET: /Admin/Packages/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Tourist Spot dropdown
            // Database se Tourist Spots la kar dropdown mein Name show hoga
            ViewBag.SpotID = new SelectList(
                await _context.TouristSpots.ToListAsync(),
                "SpotID",
                "Name"
            );

            // Hotel dropdown
            ViewBag.HotelID = new SelectList(
                await _context.Hotels.ToListAsync(),
                "HotelID",
                "Name"
            );

            // Resort dropdown
            ViewBag.ResortID = new SelectList(
                await _context.Resorts.ToListAsync(),
                "ResortID",
                "Name"
            );

            // Restaurant dropdown
            ViewBag.RestaurantID = new SelectList(
                await _context.Restaurants.ToListAsync(),
                "RestaurantID",
                "Name"
            );

            // Travel Information dropdown
            // TravelInformation model mein Title nahi,
            // isliye Route ko display text ke taur par use kar rahe hain.
            ViewBag.TravelID = new SelectList(
                await _context.TravelInformations.ToListAsync(),
                "TravelID",
                "Route"
            );

            return View();
        }


       

        // POST: /Admin/Packages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PackageID,Title,Description,Price,Discount,Duration,Nights,TotalPersons,StartDate,EndDate,Category,Inclusions,Exclusions,IsFeatured,SpotID,HotelID,ResortID,RestaurantID,TravelID,Availability")]
    Package package,
            List<IFormFile> imageFiles)
        {
            if (package.TravelID <= 0) package.TravelID = null;
            if (package.HotelID <= 0) package.HotelID = null;
            if (package.ResortID <= 0) package.ResortID = null;
            if (package.RestaurantID <= 0) package.RestaurantID = null;
            // Navigation properties ko ModelState validation se remove kar rahe hain
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("Restaurant");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("Bookings");
            ModelState.Remove("ImageUrl");

            // FIX: Agar dropdown se 0 ya unselected value aaye toh usay null kar dein taake FK conflict na ho
            if (package.TravelID == 0) package.TravelID = null;
            if (package.HotelID == 0) package.HotelID = null;
            if (package.ResortID == 0) package.ResortID = null;
            if (package.RestaurantID == 0) package.RestaurantID = null;

            // Check kar rahe hain ke selected Tourist Spot database mein exist karta hai
            var spotExists = await _context.TouristSpots
                .AnyAsync(s => s.SpotID == package.SpotID);

            if (!spotExists)
            {
                ModelState.AddModelError(
                    "SpotID",
                    "Selected Tourist Spot does not exist."
                );
            }

            // Agar sari validation successful hai
            if (ModelState.IsValid)
            {
                // Images ke paths store karne ke liye list
                List<string> imagePaths = new List<string>();

                // Agar user ne images select ki hain
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    // wwwroot/images/packages folder ka path
                    string uploadsFolder = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        "images",
                        "packages"
                    );

                    // Agar folder exist nahi karta to create karo
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Har selected image ko save karo
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            // Unique filename generate kar rahe hain
                            string uniqueFileName =
                                Guid.NewGuid().ToString()
                                + "_"
                                + Path.GetFileName(file.FileName);

                            // Complete file path
                            string filePath = Path.Combine(
                                uploadsFolder,
                                uniqueFileName
                            );

                            // File ko server par save karo
                            using (var fileStream = new FileStream(
                                filePath,
                                FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            // Database mein save hone wala relative path
                            imagePaths.Add(
                                "/images/packages/" + uniqueFileName
                            );
                        }
                    }
                }

                // Multiple image paths comma ke saath save honge
                package.ImageUrl =
                    imagePaths.Count > 0
                        ? string.Join(",", imagePaths)
                        : null;

                // Package ko database mein add karo
                _context.Packages.Add(package);

                // Database mein changes save karo
                await _context.SaveChangesAsync();

                // Package Index par wapas jao
                return RedirectToAction(nameof(Index));
            }

            // Agar validation fail ho jaye
            // to dropdowns dobara populate karne zaroori hain

            ViewBag.SpotID = new SelectList(
                await _context.TouristSpots.ToListAsync(),
                "SpotID",
                "Name",
                package.SpotID
            );

            ViewBag.HotelID = new SelectList(
                await _context.Hotels.ToListAsync(),
                "HotelID",
                "Name",
                package.HotelID
            );

            ViewBag.ResortID = new SelectList(
                await _context.Resorts.ToListAsync(),
                "ResortID",
                "Name",
                package.ResortID
            );

            ViewBag.RestaurantID = new SelectList(
                await _context.Restaurants.ToListAsync(),
                "RestaurantID",
                "Name",
                package.RestaurantID
            );

            ViewBag.TravelID = new SelectList(
                await _context.TravelInformations.ToListAsync(),
                "TravelID",
                "Route",
                package.TravelID
            );

            return View(package);
        }

        // =====================================================
        // EDIT - GET
        // =====================================================
        // GET: /Admin/Packages/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? packageid)
        {
            if (packageid == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.TouristSpot)
                .Include(p => p.Hotel)
                .Include(p => p.Resort)
                .Include(p => p.Restaurant)
                .Include(p => p.TravelInformation)
                .FirstOrDefaultAsync(p => p.PackageID == packageid);

            if (package == null)
            {
                return NotFound();
            }

            ViewBag.SpotID = new SelectList(_context.TouristSpots, "SpotID", "Name", package.SpotID);
            ViewBag.HotelID = new SelectList(_context.Hotels, "HotelID", "Name", package.HotelID);
            ViewBag.ResortID = new SelectList(_context.Resorts, "ResortID", "Name", package.ResortID);
            ViewBag.RestaurantID = new SelectList(_context.Restaurants, "RestaurantID", "Name", package.RestaurantID);
            ViewBag.TravelID = new SelectList(_context.TravelInformations, "TravelID", "Title", package.TravelID);
            return View(package);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int packageid,
            [Bind("PackageID,Title,Description,Price,Discount,Duration,Nights,TotalPersons,StartDate,EndDate,Category,Inclusions,Exclusions,IsFeatured,SpotID,HotelID,ResortID,RestaurantID,TravelID,Availability,ImageUrl")] Package package,
            List<IFormFile> imageFiles, List<string> removeImages)
        {
            // ---> Edit ke shuru mein bhi yeh check lazmi lagayein:
            if (package.TravelID <= 0) package.TravelID = null;
            if (package.HotelID <= 0) package.HotelID = null;
            if (package.ResortID <= 0) package.ResortID = null;
            if (package.RestaurantID <= 0) package.RestaurantID = null;
            if (packageid != package.PackageID)
            {
                return NotFound();
            }

            // Navigation properties form se nahi aa rahi
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("Restaurant");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("Bookings");

            // FIX: Edit mein bhi 0 ko null convert karein
            if (package.TravelID == 0) package.TravelID = null;
            if (package.HotelID == 0) package.HotelID = null;
            if (package.ResortID == 0) package.ResortID = null;
            if (package.RestaurantID == 0) package.RestaurantID = null;

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPackage = await _context.Packages.AsNoTracking().FirstOrDefaultAsync(p => p.PackageID == packageid);
                    if (existingPackage == null)
                    {
                        return NotFound();
                    }

                    List<string> currentImages = new List<string>();
                    if (!string.IsNullOrEmpty(existingPackage.ImageUrl))
                    {
                        currentImages = existingPackage.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                    }

                    // 1. User ne jin images ko remove/select kiya hai unko list aur disk dono se delete karein
                    if (removeImages != null && removeImages.Count > 0)
                    {
                        foreach (var imgToRemove in removeImages)
                        {
                            currentImages.Remove(imgToRemove);

                            // File System se delete
                            string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imgToRemove.TrimStart('/'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }

                    // 2. Nayi uploaded images ko append karein
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "packages");
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
                                currentImages.Add("/images/packages/" + uniqueFileName);
                            }
                        }
                    }

                    package.ImageUrl = string.Join(",", currentImages);

                    // Check SpotID
                    var spotExists = await _context.TouristSpots
                        .AnyAsync(s => s.SpotID == package.SpotID);

                    if (!spotExists)
                    {
                        ModelState.AddModelError(
                            "SpotID",
                            "Selected Tourist Spot does not exist."
                        );

                        ViewBag.SpotID = new SelectList(_context.TouristSpots, "SpotID", "Name", package.SpotID);
                        ViewBag.HotelID = new SelectList(_context.Hotels, "HotelID", "Name", package.HotelID);
                        ViewBag.ResortID = new SelectList(_context.Resorts, "ResortID", "Name", package.ResortID);
                        ViewBag.RestaurantID = new SelectList(_context.Restaurants, "RestaurantID", "Name", package.RestaurantID);
                        ViewBag.TravelID = new SelectList(_context.TravelInformations, "TravelID", "Route", package.TravelID);
                        return View(package);
                    }

                    _context.Packages.Update(package);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.PackageID))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.SpotID = new SelectList(_context.TouristSpots, "SpotID", "Name", package.SpotID);
            ViewBag.HotelID = new SelectList(_context.Hotels, "HotelID", "Name", package.HotelID);
            ViewBag.ResortID = new SelectList(_context.Resorts, "ResortID", "Name", package.ResortID);
            ViewBag.RestaurantID = new SelectList(_context.Restaurants, "RestaurantID", "Name", package.RestaurantID);
            ViewBag.TravelID = new SelectList(_context.TravelInformations, "TravelID", "Route", package.TravelID);
            return View(package);
        }
        // =====================================================
        // DELETE - GET
        // =====================================================
        // GET: /Admin/Packages/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? packageid)
        {
            if (packageid == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.TouristSpot)
                .Include(p => p.Hotel)
                .Include(p => p.Resort)
                .Include(p => p.Restaurant)
                .Include(p => p.TravelInformation)
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.PackageID == packageid);

            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }


        // =====================================================
        // DELETE - POST
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int packageid)
        {
            var package = await _context.Packages
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.PackageID == packageid);

            if (package == null)
            {
                return NotFound();
            }

            if (package.Bookings != null && package.Bookings.Any())
            {
                foreach (var booking in package.Bookings)
                {
                    booking.PackageID = null;
                }
            }

            // Delete associated image files from disk if present
            if (!string.IsNullOrEmpty(package.ImageUrl))
            {
                var images = package.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var imgPath in images)
                {
                    string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, imgPath.Trim().TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            _context.Packages.Remove(package);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // PACKAGE EXISTS
        // =====================================================
        private bool PackageExists(int packageid)
        {
            return _context.Packages
                .Any(p => p.PackageID == packageid);
        }
    }
}