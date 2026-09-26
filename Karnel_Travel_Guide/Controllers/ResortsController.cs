using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karnel_Travel_Guide.Data; // Apka DbContext namespace

namespace Karnel_Travel_Guide.Controllers
{
    public class ResortsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResortsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Resorts (User Side List + Search)
        public async Task<IActionResult> Index(string searchString)
        {
            var resorts = _context.Resorts.Where(r => r.Availability == true).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                resorts = resorts.Where(r => r.Name.Contains(searchString) || r.City.Contains(searchString));
            }

            return View(await resorts.ToListAsync());
        }

        // GET: /Resorts/Details/5 (User Side Details)
        public async Task<IActionResult> Details(int id)
        {
            var resort = await _context.Resorts.FirstOrDefaultAsync(m => m.ResortID == id);

            if (resort == null)
            {
                return NotFound();
            }

            return View(resort);
        }
    }
}