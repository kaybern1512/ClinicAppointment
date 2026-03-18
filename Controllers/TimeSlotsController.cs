using Microsoft.AspNetCore.Mvc;

namespace ClinicBookingMVC.Controllers
{
    public class TimeSlotsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
