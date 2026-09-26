using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;

namespace Karnel_Travel_Guide.Controllers
{
    public class TravelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TravelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Travel (User Side Listing & Filtering)
        public async Task<IActionResult> Index(string searchString, string transportType)
        {
            var query = _context.TravelInformations.Where(t => t.Availability);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(t => t.FromCity.Contains(searchString) || t.ToCity.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(transportType))
            {
                query = query.Where(t => t.TransportType == transportType);
            }

            var travels = await query.ToListAsync();
            ViewData["CurrentFilter"] = searchString;
            ViewData["TransportFilter"] = transportType;

            return View(travels);
        }

        // GET: Travel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travel = await _context.TravelInformations
                .FirstOrDefaultAsync(m => m.TravelID == id);

            if (travel == null || !travel.Availability)
            {
                return NotFound();
            }

            return View(travel);
        }
    }
}