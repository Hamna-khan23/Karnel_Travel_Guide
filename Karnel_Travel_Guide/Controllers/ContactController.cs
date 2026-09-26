using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Mvc;

namespace Karnel_Travel_Guide.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Contact
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Contact/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                contactMessage.Date = DateTime.Now;

                _context.ContactMessages.Add(contactMessage);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Your message has been sent successfully!";

                return RedirectToAction(nameof(Index));
            }

            return View(contactMessage);
        }
    }
}