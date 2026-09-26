using Microsoft.AspNetCore.Mvc;

namespace Karnel_Travel_Guide.Controllers
{
    public class InformationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}