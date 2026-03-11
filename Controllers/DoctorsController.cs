using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly ClinicBookingContext _context;

        public DoctorsController(ClinicBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? specialtyId)
        {
            var query = _context.Doctors
                .Include(d => d.Specialty)
                .Where(d => d.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.FullName.Contains(searchTerm));
            }

            if (specialtyId.HasValue && specialtyId.Value > 0)
            {
                query = query.Where(d => d.SpecialtyId == specialtyId.Value);
            }

            ViewBag.SpecialtyId = new SelectList(
                await _context.Specialties.ToListAsync(),
                "SpecialtyId",
                "SpecialtyName",
                specialtyId
            );

            ViewBag.SearchTerm = searchTerm;

            var model = await query
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
                }).ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.DoctorId == id && d.IsActive);

            if (doctor == null)
            {
                return NotFound();
            }

            var model = new DoctorViewModel
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.FullName,
                SpecialtyId = doctor.SpecialtyId,
                SpecialtyName = doctor.Specialty.SpecialtyName,
                ExperienceYears = doctor.ExperienceYears,
                Description = doctor.Description,
                ImageUrl = doctor.ImageUrl,
                WorkingTime = doctor.WorkingTime,
                IsFeatured = doctor.IsFeatured
            };

            return View(model);
        }
    }
}