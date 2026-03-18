using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingMVC.Controllers
{
    public class PaymentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Confirm()
        {
            return View();
        }
    }
}
