using Microsoft.AspNetCore.Mvc;

namespace Karnel_Travel_Guide.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
