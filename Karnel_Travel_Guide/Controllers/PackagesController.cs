using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;

namespace Karnel_Travel_Guide.Controllers
{
    public class PackagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PackagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Packages (User Side Listing)
        public async Task<IActionResult> Index()
        {
            var packages = await _context.Packages
                .Include(p => p.TouristSpot)
    .Include(p => p.Hotel)
    .Include(p => p.Resort)       
    .Include(p => p.Restaurant)   //
    .Include(p => p.Bookings)
    .Where(p => p.Availability)
    .ToListAsync();

            return View(packages);
        }

        // GET: Packages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.TouristSpot)
        .Include(p => p.Hotel)
        .Include(p => p.Resort)       
        .Include(p => p.Restaurant)   
        .Include(p => p.Bookings)
                .FirstOrDefaultAsync(m => m.PackageID == id);

            if (package == null)
            {
                return NotFound();
            }

            return View(package);
        }
    }
}