using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{[Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();

            return View(users);
        }


        // =========================
        // DETAILS - GET
        // URL: /Admin/Users/Details?userid=2
        // =========================
        [HttpGet]
        public async Task<IActionResult> Details(int? userid)
        {
            if (userid == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userid);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // =========================
        // EDIT - GET
        // URL: /Admin/Users/Edit?userid=2
        // =========================
        [HttpGet]
        public async Task<IActionResult> Edit(int? userid)
        {
            if (userid == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userid);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // =========================
        // EDIT - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int userid,
            string Name,
            string Email,
            string Role,
            string? Phone)
        {
            // Find existing user
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userid);

            if (existingUser == null)
            {
                return NotFound();
            }


            // Update user information
            existingUser.Name = Name;
            existingUser.Email = Email;
            existingUser.Role = Role;
            existingUser.Phone = Phone;


            // Save changes to database
            await _context.SaveChangesAsync();


            // Go back to Users list
            return RedirectToAction(nameof(Index));
        }


        // =========================
        // DELETE - GET
        // URL: /Admin/Users/Delete?userid=2
        // =========================
        [HttpGet]
        public async Task<IActionResult> Delete(int? userid)
        {
            if (userid == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userid);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // =========================
        // DELETE - POST
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userid)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserID == userid);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}