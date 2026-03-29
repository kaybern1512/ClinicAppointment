using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly ClinicBookingContext _context;

        public SpecialtiesController(ClinicBookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _context.Specialties
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.IsFeatured)
                .ThenBy(s => s.SpecialtyName)
                .Select(s => new SpecialtyViewModel
                {
                    SpecialtyId = s.SpecialtyId,
                    SpecialtyName = s.SpecialtyName,
                    Description = s.Description,
                    Icon = s.Icon,
                    ImageUrl = s.ImageUrl,
                    IsFeatured = s.IsFeatured
                })
                .ToListAsync();

            return View(model);
        }
    }
}