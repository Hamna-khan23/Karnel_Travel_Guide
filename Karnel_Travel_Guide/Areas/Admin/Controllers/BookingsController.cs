using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // INDEX
        // =========================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Package)
                .Include(b => b.Hotel)
                .Include(b => b.Resort)
                .Include(b => b.TouristSpot)    
        .Include(b => b.Restaurant)
                .Include(b => b.TravelInformation)
                .OrderByDescending(b => b.BookingID) 
        
                .ToListAsync();

            return View(bookings);
        }


        // =========================
        // DETAILS
        // =========================
        [HttpGet]
        public async Task<IActionResult> Details(int? bookingid)
        {
            if (bookingid == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Package)
                .Include(b => b.Hotel)
                .Include(b => b.Resort)
                .Include(b => b.TouristSpot)    
        .Include(b => b.Restaurant)
                .Include(b => b.TravelInformation)
                .FirstOrDefaultAsync(b => b.BookingID == bookingid);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        // =========================
        // DELETE - GET (Confirmation Page)
        // =========================
        [HttpGet]
        public async Task<IActionResult> Delete(int? bookingid)
        {
            if (bookingid == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Package)
                .Include(b => b.Hotel)
                .Include(b => b.Resort)
                .Include(b => b.TravelInformation)
                .Include(b => b.TouristSpot)
                .Include(b => b.Restaurant)
                .FirstOrDefaultAsync(b => b.BookingID == bookingid);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // =========================
        // DELETE - POST (Actual Database Deletion)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int bookingid)
        {
            var booking = await _context.Bookings.FindAsync(bookingid);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT - GET
        // =========================
        [HttpGet]
        public async Task<IActionResult> Edit(int? bookingid)
        {
            if (bookingid == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingID == bookingid);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }


        // =========================
        // EDIT - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int bookingid,
            int Persons,
            string Status,
            decimal TotalAmount,
            string? SpecialRequest)
        {
            // Basic validation
            if (Persons < 1 || Persons > 100)
            {
                ModelState.AddModelError("Persons",
                    "Number of persons must be between 1 and 100.");
            }

            if (TotalAmount < 0)
            {
                ModelState.AddModelError("TotalAmount",
                    "Total amount cannot be negative.");
            }

            // Allowed booking statuses
            var allowedStatuses = new[]
            {
                "Pending",
                "Confirmed",
                "Completed",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(Status))
            {
                ModelState.AddModelError("Status",
                    "Invalid booking status.");
            }

            // Agar validation fail ho
            if (!ModelState.IsValid)
            {
                var bookingForView = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingID == bookingid);

                if (bookingForView == null)
                {
                    return NotFound();
                }

                bookingForView.Persons = Persons;
                bookingForView.Status = Status;
                bookingForView.TotalAmount = TotalAmount;
                bookingForView.SpecialRequest = SpecialRequest;

                return View(bookingForView);
            }


            // Existing booking database se nikalo
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingID == bookingid);

            if (existingBooking == null)
            {
                return NotFound();
            }


            // =========================
            // UPDATE ONLY EDITABLE FIELDS
            // =========================

            existingBooking.Persons = Persons;

            existingBooking.Status = Status;

            existingBooking.TotalAmount = TotalAmount;

            existingBooking.SpecialRequest = SpecialRequest;


            // =========================
            // SAVE CHANGES
            // =========================

            await _context.SaveChangesAsync();


            // Wapas bookings list
            return RedirectToAction(nameof(Index));
        }


        // =========================
        // CHECK BOOKING EXISTS
        // =========================
        private bool BookingExists(int bookingid)
        {
            return _context.Bookings
                .Any(b => b.BookingID == bookingid);
        }
    }
}