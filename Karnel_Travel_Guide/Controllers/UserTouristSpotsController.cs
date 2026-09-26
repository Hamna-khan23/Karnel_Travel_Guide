using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;

namespace Karnel_Travel_Guide.Controllers
{
    public class UserTouristSpotsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserTouristSpotsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserTouristSpots (Public Listing + Search & Category Filter)
        public async Task<IActionResult> Index(string searchString, string city, string category)
        {
            var spotsQuery = _context.TouristSpots
                .Include(t => t.Packages)
                .AsQueryable();

            // Search by Spot Name or Description
            if (!string.IsNullOrEmpty(searchString))
            {
                spotsQuery = spotsQuery.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }

            // Filter by City
            if (!string.IsNullOrEmpty(city))
            {
                spotsQuery = spotsQuery.Where(s => s.City == city);
            }

            // Filter by Category
            if (!string.IsNullOrEmpty(category))
            {
                spotsQuery = spotsQuery.Where(s => s.Category == category);
            }

            // Dropdown filters ke liye ViewBag
            ViewBag.Cities = await _context.TouristSpots.Select(s => s.City).Distinct().ToListAsync();
            ViewBag.Categories = await _context.TouristSpots.Select(s => s.Category).Distinct().ToListAsync();

            var spots = await spotsQuery.ToListAsync();
            return View(spots);
        }

        // GET: UserTouristSpots/Details/5 (Public Spot Details + Related Packages)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var touristspot = await _context.TouristSpots
                .Include(t => t.Packages)
                .FirstOrDefaultAsync(m => m.SpotID == id);

            if (touristspot == null)
            {
                return NotFound();
            }

            return View(touristspot);
        }
    }
}