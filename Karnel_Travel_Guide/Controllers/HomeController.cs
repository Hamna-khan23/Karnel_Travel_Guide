using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Karnel_Travel_Guide.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _context.Packages
                .Include(p => p.TouristSpot)
                .Where(p => p.Availability)
                .OrderByDescending(p => p.PackageID)
                .Take(3)
                .ToListAsync();

            ViewBag.Travels = await _context.TravelInformations
         .OrderByDescending(t => t.TravelID)
         .Take(3)
         .ToListAsync();


            ViewBag.Resorts = await _context.Resorts.Take(3).ToListAsync();

            return View(packages);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id
                                ?? HttpContext.TraceIdentifier
                });
        }
    }
}