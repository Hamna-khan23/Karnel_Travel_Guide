

using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RestaurantsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: RESTAURANTS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Restaurants.ToListAsync());
        }

        // GET: RESTAURANTS/Details/5
        public async Task<IActionResult> Details(int? restaurantid)
        {
            if (restaurantid == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.RestaurantID == restaurantid);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // GET: RESTAURANTS/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RESTAURANTS/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Restaurant restaurant, List<IFormFile> imageFiles)
        {
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                List<string> imagePaths = new List<string>();

                if (imageFiles != null && imageFiles.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "restaurants");
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

                            imagePaths.Add("/images/restaurants/" + uniqueFileName);
                        }
                    }
                }

                restaurant.ImageUrl = string.Join(",", imagePaths);

                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(restaurant);
        }

        // GET: RESTAURANTS/Edit/5
        public async Task<IActionResult> Edit(int? restaurantid)
        {
            if (restaurantid == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(restaurantid);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        // POST: RESTAURANTS/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? restaurantid, Restaurant restaurant, List<IFormFile> imageFiles, List<string> removeImages)
        {
            if (restaurantid != restaurant.RestaurantID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRestaurant = await _context.Restaurants.AsNoTracking().FirstOrDefaultAsync(x => x.RestaurantID == restaurantid);
                    if (existingRestaurant == null) return NotFound();

                    List<string> currentImages = new List<string>();
                    if (!string.IsNullOrEmpty(existingRestaurant.ImageUrl))
                    {
                        currentImages = existingRestaurant.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList();
                    }

                    // 1. Remove selected images from list and disk
                    if (removeImages != null && removeImages.Count > 0)
                    {
                        foreach (var imgToRemove in removeImages)
                        {
                            currentImages.Remove(imgToRemove);

                            string fullPath = Path.Combine(_environment.WebRootPath, imgToRemove.TrimStart('/'));
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }

                    // 2. Append newly uploaded images
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "restaurants");
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
                                currentImages.Add("/images/restaurants/" + uniqueFileName);
                            }
                        }
                    }

                    restaurant.ImageUrl = string.Join(",", currentImages);

                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.RestaurantID))
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
            return View(restaurant);
        }

        // GET: RESTAURANTS/Delete/5
        public async Task<IActionResult> Delete(int? restaurantid)
        {
            if (restaurantid == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants
                .FirstOrDefaultAsync(m => m.RestaurantID == restaurantid);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: RESTAURANTS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? restaurantid)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantid);
            if (restaurant != null)
            {
                if (!string.IsNullOrEmpty(restaurant.ImageUrl))
                {
                    var images = restaurant.ImageUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var imgPath in images)
                    {
                        string fullPath = Path.Combine(_environment.WebRootPath, imgPath.Trim().TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }

                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(int? restaurantid)
        {
            return _context.Restaurants.Any(e => e.RestaurantID == restaurantid);
        }
    }
}