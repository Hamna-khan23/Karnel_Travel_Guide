using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ContactMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: /Admin/ContactMessages
        // Shows all contact messages
        // ==========================================
        public async Task<IActionResult> Index()
        {
            return View(await _context.ContactMessages
                .OrderByDescending(m => m.Date)
                .ToListAsync());
        }


        // ==========================================
        // GET: /Admin/ContactMessages/Details/5
        // Shows one complete message
        // ==========================================
        public async Task<IActionResult> Details(int? messageid)
        {
            if (messageid == null)
            {
                return NotFound();
            }

            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.MessageID == messageid);

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }


        // ==========================================
        // GET: /Admin/ContactMessages/Delete/5
        // Shows delete confirmation
        // ==========================================
        public async Task<IActionResult> Delete(int? messageid)
        {
            if (messageid == null)
            {
                return NotFound();
            }

            var message = await _context.ContactMessages
                .FirstOrDefaultAsync(m => m.MessageID == messageid);

            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }


        // ==========================================
        // POST: /Admin/ContactMessages/Delete/5
        // Deletes the message
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int messageid)
        {
            var message = await _context.ContactMessages
                .FindAsync(messageid);

            if (message != null)
            {
                _context.ContactMessages.Remove(message);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // Checks whether message exists
        // ==========================================
        private bool MessageExists(int messageid)
        {
            return _context.ContactMessages
                .Any(m => m.MessageID == messageid);
        }
    }
}