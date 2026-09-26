
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Karnel_Travel_Guide.Controllers
{
    public class FeedbkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbkController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: /Feedbk/Create
        // =========================================================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //// Get logged-in user's UserID from Claim
            //var userIdString = User.FindFirstValue(
            //    ClaimTypes.NameIdentifier
            //);

            //if (!int.TryParse(userIdString, out int userId))
            //{
            //    return RedirectToAction("Login", "Accounts");
            //}

            //// Get only this user's bookings
            //var bookings = await _context.Bookings
            //    .Where(b => b.UserID == userId)
            //    .OrderByDescending(b => b.BookingDate)
            //    .ToListAsync();

            //ViewBag.Bookings = new SelectList(
            //    bookings,
            //    "BookingID",
            //    "BookingID"
            //);

            return View();
        }


        // =========================================================
        // POST: /Feedbk/Create
        // =========================================================
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            // Get logged-in user's UserID from Claim
            var userIdString = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "Accounts");
            }

            // IMPORTANT:
            // Never trust UserID coming from the form.
            // Set it from the logged-in user's claim.
            feedback.UserID = userId;

            // Navigation properties are not entered in the form
            ModelState.Remove("User");
            ModelState.Remove("Booking");


            // =====================================================
            // CHECK 1:
            // Does the selected booking belong to this user?
            // =====================================================
            var bookingExists = await _context.Bookings
                .AnyAsync(b =>
                    b.BookingID == feedback.BookingID &&
                    b.UserID == userId
                );

            if (!bookingExists)
            {
                ModelState.AddModelError(
                    "BookingID",
                    "The selected booking does not belong to your account."
                );
            }


            


            // =====================================================
            // VALIDATION FAILED
            // =====================================================
            if (!ModelState.IsValid)
            {
                // Reload user's bookings for dropdown
                var bookings = await _context.Bookings
                    .Where(b => b.UserID == userId)
                    .OrderByDescending(b => b.BookingDate)
                    .ToListAsync();

                ViewBag.Bookings = new SelectList(
                    bookings,
                    "BookingID",
                    "BookingID",
                    feedback.BookingID
                );

                return View(feedback);
            }


            // =====================================================
            // SET FEEDBACK DATE
            // =====================================================
            feedback.Date = DateTime.Now;


            // =====================================================
            // SAVE FEEDBACK
            // =====================================================
            _context.Feedbacks.Add(feedback);

            await _context.SaveChangesAsync();


            // =====================================================
            // SUCCESS MESSAGE
            // =====================================================
            TempData["SuccessMessage"] =
                "Thank you! Your feedback has been submitted successfully.";


            return RedirectToAction(nameof(Create));
        }
    }
}
