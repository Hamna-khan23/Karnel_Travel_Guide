using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Controllers
{
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HotelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Hotels
        public async Task<IActionResult> Index(string searchString, string city)
        {
            var hotelsQuery = _context.Hotels.AsQueryable();

            // Search Filter
            if (!string.IsNullOrEmpty(searchString))
            {
                hotelsQuery = hotelsQuery.Where(h => h.Name.Contains(searchString) || h.City.Contains(searchString));
            }

            // City Filter
            if (!string.IsNullOrEmpty(city))
            {
                hotelsQuery = hotelsQuery.Where(h => h.City.Contains(city));
            }

            var hotels = await hotelsQuery.ToListAsync();
            return View(hotels);
        }

        // GET: /Hotels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hotels.FirstOrDefaultAsync(m => m.HotelID == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }
    }
}