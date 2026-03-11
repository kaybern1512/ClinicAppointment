using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClinicBookingContext _context;

        public HomeController(ClinicBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                FeaturedSpecialties = await _context.Specialties
                    .Where(s => s.IsFeatured)
                    .Select(s => new SpecialtyViewModel
                    {
                        SpecialtyId = s.SpecialtyId,
                        SpecialtyName = s.SpecialtyName,
                        Description = s.Description,
                        Icon = s.Icon,
                        ImageUrl = s.ImageUrl,
                        IsFeatured = s.IsFeatured
                    }).ToListAsync(),

                FeaturedDoctors = await _context.Doctors
                    .Include(d => d.Specialty)
                    .Where(d => d.IsFeatured && d.IsActive)
                    .Select(d => new DoctorViewModel
                    {
                        DoctorId = d.DoctorId,
                        FullName = d.FullName,
                        SpecialtyId = d.SpecialtyId,
                        SpecialtyName = d.Specialty.SpecialtyName,
                        ExperienceYears = d.ExperienceYears,
                        Description = d.Description,
                        ImageUrl = d.ImageUrl,
                        WorkingTime = d.WorkingTime,
                        IsFeatured = d.IsFeatured
                    }).ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = new ContactMessage
            {
                FullName = model.FullName,
                Email = model.Email,
                Message = model.Message,
                SentAt = DateTime.Now
            };

            _context.ContactMessages.Add(entity);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gửi liên hệ thành công.";
            ModelState.Clear();

            return View(new ContactViewModel());
        }
    }
}