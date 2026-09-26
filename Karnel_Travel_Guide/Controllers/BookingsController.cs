using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Karnel_Travel_Guide.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            // 1. Sabse pehle seedha NameIdentifier (UserID) Claim se nikalein
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int parsedUserId) && parsedUserId > 0)
            {
                // Check karein kya yeh ID database mein mojood hai?
                if (_context.Users.Any(u => u.UserID == parsedUserId))
                {
                    return parsedUserId;
                }
            }

            // 2. Agar Claim na mile to Email Claim se dhoondein
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var dbUserByEmail = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (dbUserByEmail != null)
                {
                    return dbUserByEmail.UserID;
                }
            }

            // 3. Agar abhi bhi na mile to Identity Name se dhoondein
            var userName = User.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                var dbUser = _context.Users.FirstOrDefault(u => u.Name == userName || u.Email == userName);
                if (dbUser != null)
                {
                    return dbUser.UserID;
                }
            }

            // 4. Emergency Fallback: Agar cookie purani ho aur user 0 ban raha ho, 
            // to Database ka pehla active user utha lo taake SQL crash na ho:
            var firstUser = _context.Users.FirstOrDefault();
            return firstUser != null ? firstUser.UserID : 1;
        }

        [HttpGet]
        public async Task<IActionResult> BookPackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);

            if (package == null)
                return NotFound();

            decimal origPrice = package.Price;
            decimal disc = package.Discount;

            decimal finalPrice = disc > 0
                ? origPrice - (origPrice * disc / 100)
                : origPrice;

            ViewBag.ItemName = package.Title;
            ViewBag.BasePrice = finalPrice;
            ViewBag.BasePersons = package.TotalPersons > 0 ? package.TotalPersons : 2;

            // Dates ViewBag mein pass karein display ke liye
            ViewBag.StartDate = package.StartDate;
            ViewBag.EndDate = package.EndDate;

            var booking = new Booking
            {
                UserID = GetCurrentUserId(),
                PackageID = package.PackageID,
                BookingDate = DateTime.Now,
                CheckInDate = package.StartDate ?? DateTime.Now,
                CheckOutDate = package.EndDate ?? DateTime.Now.AddDays(package.Nights > 0 ? package.Nights : 1),
                Persons = package.TotalPersons > 0 ? package.TotalPersons : 2,
                Status = "Pending",
                Nights = package.Nights
            };

            return View(booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookPackage(Booking booking)
        {
            booking.UserID = GetCurrentUserId();
            booking.Status = "Pending";
            booking.BookingDate = DateTime.Now;

            var package = await _context.Packages.FindAsync(booking.PackageID);

            if (package == null)
                return NotFound();
       
            booking.CheckInDate = package.StartDate ?? DateTime.Now;
            booking.CheckOutDate = package.EndDate ?? DateTime.Now.AddDays(package.Nights > 0 ? package.Nights : 1);
            booking.Nights = package.Nights;

            decimal originalPrice = package.Price;
            decimal discount = package.Discount;

            decimal finalPrice = discount > 0
                ? originalPrice - (originalPrice * discount / 100)
                : originalPrice;

            int basePersons = package.TotalPersons > 0 ? package.TotalPersons : 2;
            int selectedPersons = booking.Persons > 0 ? booking.Persons : basePersons;

            booking.TotalAmount = (finalPrice / basePersons) * selectedPersons;

            // Navigation properties ko ModelState validation se remove karein
            ModelState.Remove("User");
            ModelState.Remove("Package");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Restaurant");

            ModelState.Remove("SpecialRequest");
            ModelState.Remove("PriceRange");
            ModelState.Remove("Status");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("Nights");

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation));
            }

            ViewBag.ItemName = package.Title;
            ViewBag.BasePrice = finalPrice;
            ViewBag.BasePersons = basePersons;
            ViewBag.StartDate = package.StartDate;
            ViewBag.EndDate = package.EndDate;

            return View("BookPackage", booking);
        }
        
        // ==================== 2. HOTEL / RESORT BOOKING FLOW (Per Night Calculation) ====================

        [HttpGet]
        public async Task<IActionResult> BookAccommodation(int? hotelId, int? resortId)
        {
            var booking = new Booking
            {
                UserID = GetCurrentUserId(),
                BookingDate = DateTime.Now,
                Persons = 1,
                Nights = 1,
                Status = "Pending",
                HotelID = hotelId,
                ResortID = resortId
            };

            decimal pricePerNight = 0;
            if (hotelId.HasValue)
            {
                var hotel = await _context.Hotels.FindAsync(hotelId);
                if (hotel == null) return NotFound();
                ViewBag.ItemName = hotel.Name;
                pricePerNight = hotel.PricePerNight;
            }
            else if (resortId.HasValue)
            {
                var resort = await _context.Resorts.FindAsync(resortId);
                if (resort == null) return NotFound();
                ViewBag.ItemName = resort.Name;
                pricePerNight = resort.PricePerNight;
            }
            else
            {
                return NotFound();
            }

            ViewBag.PricePerNight = pricePerNight;
            return View(booking);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAccommodation(
      Booking booking,
      int? hotelId,
      int? resortId)
        {
            // Route/Form IDs assign karein
            if (!booking.HotelID.HasValue && hotelId.HasValue)
            {
                booking.HotelID = hotelId;
            }

            if (!booking.ResortID.HasValue && resortId.HasValue)
            {
                booking.ResortID = resortId;
            }

            // Current logged-in user
            booking.UserID = GetCurrentUserId();
            booking.Status = "Pending";

            decimal pricePerNight = 0;

            // =========================
            // HOTEL
            // =========================
            if (booking.HotelID.HasValue)
            {
                var hotel = await _context.Hotels
                    .FirstOrDefaultAsync(h => h.HotelID == booking.HotelID.Value);

                if (hotel == null) return NotFound();

                pricePerNight = hotel.PricePerNight;
                ViewBag.ItemName = hotel.Name;
            }
            // =========================
            // RESORT
            // =========================
            else if (booking.ResortID.HasValue)
            {
                var resort = await _context.Resorts
                    .FirstOrDefaultAsync(r => r.ResortID == booking.ResortID.Value);

                if (resort == null) return NotFound();

                pricePerNight = resort.PricePerNight;
                ViewBag.ItemName = resort.Name;
            }
            else
            {
                return BadRequest("Hotel or Resort was not selected.");
            }

            // =========================
            // DATE VALIDATION
            if (booking.CheckInDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    "CheckInDate",
                    "Check-In date cannot be in the past. Please select today or a future date."
                );
            }
            else if (booking.CheckOutDate.Date <= booking.CheckInDate.Date)
            {
                ModelState.AddModelError(
                    "CheckOutDate",
                    "Check-Out date must be after Check-In date."
                );
            }
            else
            {
                booking.Nights =
                    (booking.CheckOutDate.Date - booking.CheckInDate.Date).Days;
            }
            // =========================
            // PRICE CALCULATION
            // =========================
            booking.TotalAmount = pricePerNight * booking.Nights;

            // ========================================================
            // MODELSTATE REMOVALS (Yeh sab remove karna zaroori hai)
            // ========================================================
            ModelState.Remove("User");
            ModelState.Remove("Package");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Restaurant");

            // Zaroori Fixes:
            ModelState.Remove("PriceRange");
            ModelState.Remove("SpecialRequest"); // <-- Yeh missing tha
            ModelState.Remove("Status");         // <-- Form se nahi aa raha
            ModelState.Remove("TotalAmount");    // <-- Controller calculate kar raha hai
            ModelState.Remove("Nights");         // <-- Controller calculate kar raha hai

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation));
            }
            // Current logged-in user
            booking.UserID = GetCurrentUserId();

            // AGAR USERID PHIR BHI 0 RAHE TO LOGIN PAR BHEJEIN:
            if (booking.UserID == 0)
            {
                return RedirectToAction("Login", "Accounts", new { returnUrl = Request.Path });
            }
            // Agar validation abhi bhi fail ho to ViewBag values wapas bhejein
            ViewBag.PricePerNight = pricePerNight;
            ViewBag.ItemName = booking.HotelID.HasValue ? ViewBag.ItemName : ViewBag.ItemName;

            return View(booking);

        }

        // ==================== 3. TOURIST SPOT BOOKING FLOW (Per Person Calculation) ====================
        [HttpGet]
        public async Task<IActionResult> BookTouristSpot(int id)
        {
            var spot = await _context.TouristSpots.FindAsync(id);

            if (spot == null)
                return NotFound();

            ViewBag.ItemName = spot.Name;
            ViewBag.PricePerPerson = spot.PricePerNight;
            ViewBag.BookingAction = "BookTouristSpot";

            var booking = new Booking
            {
                UserID = GetCurrentUserId(),
                TouristSpotID = id,
                BookingDate = DateTime.Now,
                Persons = 1,
                Status = "Pending",
                Nights = 0
            };

            return View("~/Views/Bookings/BookTouristSpotOrTravel.cshtml", booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookTouristSpot(Booking booking)
        {
            booking.UserID = GetCurrentUserId();
            booking.Status = "Pending";
            booking.Nights = 0;
            booking.BookingDate = DateTime.Now;

            var spot = await _context.TouristSpots.FindAsync(booking.TouristSpotID);

            if (spot == null)
                return NotFound();

            int persons = booking.Persons > 0 ? booking.Persons : 1;

            // Tourist Spot = Price Per Person × Persons
            booking.TotalAmount = spot.PricePerNight * persons;

            // Navigation properties ModelState validation se remove
            ModelState.Remove("User");
            ModelState.Remove("Package");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Restaurant");
            ModelState.Remove("PriceRange");

            ModelState.Remove("SpecialRequest");
            ModelState.Remove("Status");           
            ModelState.Remove("TotalAmount");      
            ModelState.Remove("CheckOutDate");

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation));
            }

            ViewBag.ItemName = spot.Name;
            ViewBag.PricePerPerson = spot.PricePerNight;
            ViewBag.BookingAction = "BookTouristSpot";

            return View("BookTouristSpotOrTravel", booking);
        }

        // ==================== 4. TRAVEL INFORMATION BOOKING FLOW (Per Person Calculation) ====================
        [HttpGet]
        public async Task<IActionResult> BookTravel(int id)
        {
            var travel = await _context.TravelInformations.FindAsync(id);

            if (travel == null)
                return NotFound();

            ViewBag.ItemName = travel.TransportType;
            ViewBag.PricePerPerson = travel.Price;
            ViewBag.BookingAction = "BookTravel";

            var booking = new Booking
            {
                UserID = GetCurrentUserId(),
                TravelID = id,
                BookingDate = DateTime.Now,
                Persons = 1,
                Status = "Pending",
                Nights = 0
            };

            return View("~/Views/Bookings/BookTouristSpotOrTravel.cshtml", booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookTravel(Booking booking)
        {
            booking.UserID = GetCurrentUserId();
            booking.Status = "Pending";
            booking.Nights = 0;
            booking.BookingDate = DateTime.Now;

            var travel = await _context.TravelInformations
                .FindAsync(booking.TravelID);

            if (travel == null)
                return NotFound();

            int persons = booking.Persons > 0 ? booking.Persons : 1;

            // Travel = Price Per Person × Persons
            booking.TotalAmount = travel.Price * persons;

            // Navigation properties ModelState validation se remove
            ModelState.Remove("User");
            ModelState.Remove("Package");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Restaurant");
            ModelState.Remove("PriceRange");

            ModelState.Remove("SpecialRequest");
           
            ModelState.Remove("Status");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("CheckOutDate");

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation));
            }

            ViewBag.ItemName = travel.TransportType;
            ViewBag.PricePerPerson = travel.Price;
            ViewBag.BookingAction = "BookTravel";

            return View("BookTouristSpotOrTravel", booking);
        }

        // ==================== 5. RESTAURANT RESERVATION FLOW (No Calculation) ====================
        [HttpGet]
        public async Task<IActionResult> BookRestaurant(int id)
        {
            var restaurant = await _context.Restaurants
                .FindAsync(id);

            if (restaurant == null)
                return NotFound();

            ViewBag.ItemName = restaurant.Name;
            ViewBag.PriceRange = restaurant.PriceRange;

            var booking = new Booking
            {
                UserID = GetCurrentUserId(),
                RestaurantID = restaurant.RestaurantID,
                BookingDate = DateTime.Now,
                Persons = 1,
                Status = "Reserved",
                Nights = 0
            };

            return View(booking);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookRestaurant(Booking booking)
        {
            booking.UserID = GetCurrentUserId();
            booking.Status = "Reserved";
            booking.Nights = 0;
            booking.BookingDate = DateTime.Now;

            // Restaurant trip sirf usi din ke liye hoti hai
            booking.CheckOutDate = booking.CheckInDate;

            // Restaurant find karein
            var restaurant = await _context.Restaurants
                .FindAsync(booking.RestaurantID);

            if (restaurant == null)
                return NotFound();

            // Restaurant ka PriceRange Booking table mein save hoga
            booking.PriceRange = restaurant.PriceRange;

            // Navigation properties ko ModelState validation se remove karein
            ModelState.Remove("User");
            ModelState.Remove("Package");
            ModelState.Remove("Hotel");
            ModelState.Remove("Resort");
            ModelState.Remove("TravelInformation");
            ModelState.Remove("TouristSpot");
            ModelState.Remove("Restaurant");

            // Ye sari fields validation se remove karein:
            ModelState.Remove("SpecialRequest");
            ModelState.Remove("PriceRange");
            ModelState.Remove("Status");
            ModelState.Remove("TotalAmount");
            ModelState.Remove("CheckOutDate");
            ModelState.Remove("Nights");

            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Confirmation));
            }

            // Validation fail ho to restaurant ki information dobara bhejein
            ViewBag.ItemName = restaurant.Name;
            ViewBag.PriceRange = restaurant.PriceRange;

            return View(booking);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> BookRestaurant(Booking booking)
        //{
        //    booking.UserID = GetCurrentUserId();
        //    booking.Status = "Reserved";
        //    booking.Nights = 0;
        //    booking.BookingDate = DateTime.Now;

        //    // Restaurant find karein
        //    var restaurant = await _context.Restaurants
        //        .FindAsync(booking.RestaurantID);

        //    if (restaurant == null)
        //        return NotFound();

        //    // Restaurant ka PriceRange Booking table mein save hoga
        //    booking.PriceRange = restaurant.PriceRange;

        //    // Navigation properties ko ModelState validation se remove karein
        //    ModelState.Remove("User");
        //    ModelState.Remove("Package");
        //    ModelState.Remove("Hotel");
        //    ModelState.Remove("Resort");
        //    ModelState.Remove("TravelInformation");
        //    ModelState.Remove("TouristSpot");
        //    ModelState.Remove("Restaurant");
        //    ModelState.Remove("SpecialRequest");

        //    if (ModelState.IsValid)
        //    {
        //        _context.Bookings.Add(booking);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Confirmation));
        //    }

        //    // Validation fail ho to restaurant ki information dobara bhejein
        //    ViewBag.ItemName = restaurant.Name;
        //    ViewBag.PriceRange = restaurant.PriceRange;

        //    return View("BookRestaurant", booking);
        //}
        public IActionResult Confirmation() => View();

        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            int userId = GetCurrentUserId();

            var userBookings = await _context.Bookings
                .Include(b => b.Hotel)
                .Include(b => b.Resort)
                .Include(b => b.Package)
                .Include(b => b.TravelInformation)
                .Include(b => b.TouristSpot)
                .Include(b => b.Restaurant)
                .Where(b => b.UserID == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(userBookings);
        }
    }
}