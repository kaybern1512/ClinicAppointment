using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingMVC.Controllers
{
    public class RelativesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
