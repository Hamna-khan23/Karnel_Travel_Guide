using Karnel_Travel_Guide.Data;
using Karnel_Travel_Guide.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Karnel_Travel_Guide.Controllers
{
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // REGISTER - GET
        // =========================================================
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }


        // =========================================================
        // REGISTER - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            User user,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            // Navigation properties are not required during registration
            ModelState.Remove("Bookings");
            ModelState.Remove("Feedbacks");

            // Check duplicate email
            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    "An account with this email already exists."
                );
            }

            if (ModelState.IsValid)
            {
                // =================================================
                // NORMAL REGISTRATION = USER ONLY
                // =================================================
                user.Role = "User";
                user.CreatedDate = DateTime.Now;

                _context.Users.Add(user);

                await _context.SaveChangesAsync();


                // =================================================
                // CREATE USER CLAIMS
                // =================================================
                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.Name,
                        user.Name
                    ),

                    new Claim(
                        ClaimTypes.Email,
                        user.Email
                    ),

                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.UserID.ToString()
                    ),

                    new Claim(
                        ClaimTypes.Role,
                        "User"
                    )
                };


                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var principal = new ClaimsPrincipal(identity);


                // =================================================
                // SIGN IN USER
                // =================================================
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );


                // =================================================
                // RETURN URL
                // =================================================
                if (!string.IsNullOrEmpty(returnUrl)
                    && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }


                // =================================================
                // NORMAL USER → HOME
                // =================================================
                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }


            // Validation failed
            return View(user);
        }


        // =========================================================
        // LOGIN - GET
        // =========================================================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }


        // =========================================================
        // LOGIN - POST
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string email,
            string password,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;


            // =================================================
            // FIND USER
            // =================================================
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.PasswordHash == password
                );


            // =================================================
            // INVALID LOGIN
            // =================================================
            if (user == null)
            {
                ViewBag.Error = "Invalid Email or Password.";

                return View();
            }


            // =================================================
            // GET ROLE
            // =================================================
            string role = string.IsNullOrEmpty(user.Role)
                ? "User"
                : user.Role;


            // =================================================
            // CREATE CLAIMS
            // =================================================
            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Name
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()
                ),

                new Claim(
                    ClaimTypes.Role,
                    role
                )
            };


            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);


            // =================================================
            // SIGN IN
            // =================================================
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );


            // =================================================
            // ADMIN
            // =================================================
            if (role.Equals(
                "Admin",
                StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(returnUrl)
                    && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new { area = "Admin" }
                );
            }


            // =================================================
            // NORMAL USER
            // =================================================
            if (!string.IsNullOrEmpty(returnUrl)
                && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }


            return RedirectToAction(
                "Index",
                "Home"
            );
        }


        // =========================================================
        // LOGOUT
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }
}