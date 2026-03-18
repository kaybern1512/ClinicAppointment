using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingMVC.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
