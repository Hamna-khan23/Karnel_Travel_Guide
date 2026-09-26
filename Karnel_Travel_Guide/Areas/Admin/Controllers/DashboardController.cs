//using Karnel_Travel_Guide.Data;
//using Karnel_Travel_Guide.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Karnel_Travel_Guide.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    [Authorize]

//    public class DashboardController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public DashboardController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var dashboard = new DashboardViewModel
//            {
//                TotalUsers = await _context.Users.CountAsync(),

//                TotalHotels = await _context.Hotels.CountAsync(),

//                TotalTouristSpots = await _context.TouristSpots.CountAsync(),

//                TotalResorts = await _context.Resorts.CountAsync(),

//                TotalRestaurants = await _context.Restaurants.CountAsync(),

//                TotalTravelInformation = await _context.TravelInformations.CountAsync(),

//                TotalPackages = await _context.Packages.CountAsync(),

//                TotalBookings = await _context.Bookings.CountAsync(),

//                TotalFeedback = await _context.Feedbacks.CountAsync(),

//                TotalContactMessages = await _context.ContactMessages.CountAsync()
//            };

//            return View(dashboard);
//        }
//    }
//}
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),

                TotalHotels = await _context.Hotels.CountAsync(),

                TotalTouristSpots = await _context.TouristSpots.CountAsync(),

                TotalResorts = await _context.Resorts.CountAsync(),

                TotalRestaurants = await _context.Restaurants.CountAsync(),

                TotalTravelInformation = await _context.TravelInformations.CountAsync(),

                TotalPackages = await _context.Packages.CountAsync(),

                TotalBookings = await _context.Bookings.CountAsync(),

                TotalFeedback = await _context.Feedbacks.CountAsync(),

                TotalContactMessages = await _context.ContactMessages.CountAsync()
            };

            return View(dashboard);
        }
    }
}