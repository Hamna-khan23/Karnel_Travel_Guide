using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authorization;

namespace Karnel_Travel_Guide.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // GET: /Admin/Feedbacks
        // Shows all feedback
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .ToListAsync();

            return View(feedbacks);
        }


        // ==========================================
        // GET: /Admin/Feedbacks/Details/5
        // Shows details of one feedback
        // ==========================================
        public async Task<IActionResult> Details(int? feedbackid)
        {
            // Check whether FeedbackID was provided
            if (feedbackid == null)
            {
                return NotFound();
            }

            // Find feedback and its related User
            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FeedbackID == feedbackid);

            // If feedback does not exist
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }


        // ==========================================
        // GET: /Admin/Feedbacks/Delete/5
        // Shows delete confirmation page
        // ==========================================
        public async Task<IActionResult> Delete(int? feedbackid)
        {
            // Check whether FeedbackID was provided
            if (feedbackid == null)
            {
                return NotFound();
            }

            // Find feedback and related User
            var feedback = await _context.Feedbacks
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FeedbackID == feedbackid);

            // If feedback does not exist
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }


        // ==========================================
        // POST: /Admin/Feedbacks/Delete/5
        // Deletes the selected feedback
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int feedbackid)
        {
            // Find feedback by ID
            var feedback = await _context.Feedbacks
                .FindAsync(feedbackid);

            // If feedback exists, delete it
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);

                await _context.SaveChangesAsync();
            }

            // Go back to feedback list
            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // Checks whether feedback exists
        // ==========================================
        private bool FeedbackExists(int feedbackid)
        {
            return _context.Feedbacks
                .Any(f => f.FeedbackID == feedbackid);
        }
    }
}