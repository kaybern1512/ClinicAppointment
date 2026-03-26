using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminDoctorsController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminDoctorsController(ClinicBookingContext context)
        {
            _context = context;
        }

        private async Task LoadSpecialtiesAsync(object model)
        {
            var specialties = await _context.Specialties
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Select(s => new SelectListItem
                {
                    Value = s.SpecialtyId.ToString(),
                    Text = s.SpecialtyName
                })
                .ToListAsync();

            if (model is AdminDoctorCreateViewModel createModel)
                createModel.Specialties = specialties;

            if (model is AdminDoctorEditViewModel editModel)
                editModel.Specialties = specialties;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm, int? specialtyId)
        {
            searchTerm = searchTerm?.Trim();

            var query = _context.Doctors
                .AsNoTracking()
                .Include(d => d.Specialty)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.FullName.Contains(searchTerm));
            }

            if (specialtyId.HasValue && specialtyId > 0)
            {
                query = query.Where(d => d.SpecialtyId == specialtyId);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SpecialtyId = new SelectList(
                await _context.Specialties.AsNoTracking().Where(s => s.IsActive).ToListAsync(),
                "SpecialtyId",
                "SpecialtyName",
                specialtyId
            );

            var model = await query
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new AdminDoctorListItemViewModel
                {
                    DoctorId = d.DoctorId,
                    FullName = d.FullName,
                    SpecialtyName = d.Specialty.SpecialtyName,
                    ExperienceYears = d.ExperienceYears,
                    ImageUrl = d.ImageUrl,
                    IsFeatured = d.IsFeatured,
                    IsActive = d.IsActive
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminDoctorCreateViewModel
            {
                IsActive = true,
                IsFeatured = false
            };

            await LoadSpecialtiesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminDoctorCreateViewModel model)
        {
            model.FullName = model.FullName?.Trim();

            if (!ModelState.IsValid)
            {
                await LoadSpecialtiesAsync(model);
                return View(model);
            }

            var doctor = new Doctor
            {
                FullName = model.FullName!,
                SpecialtyId = model.SpecialtyId,
                ExperienceYears = model.ExperienceYears,
                Description = model.Description,
                ImageUrl = model.ImageUrl,
                WorkingTime = model.WorkingTime,
                IsFeatured = model.IsFeatured,
                IsActive = model.IsActive,
                CreatedAt = DateTime.Now
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm bác sĩ thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            var model = new AdminDoctorEditViewModel
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.FullName,
                SpecialtyId = doctor.SpecialtyId,
                ExperienceYears = doctor.ExperienceYears,
                Description = doctor.Description,
                ImageUrl = doctor.ImageUrl,
                WorkingTime = doctor.WorkingTime,
                IsFeatured = doctor.IsFeatured,
                IsActive = doctor.IsActive
            };

            await LoadSpecialtiesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminDoctorEditViewModel model)
        {
            model.FullName = model.FullName?.Trim();

            var doctor = await _context.Doctors.FindAsync(model.DoctorId);
            if (doctor == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadSpecialtiesAsync(model);
                return View(model);
            }

            doctor.FullName = model.FullName!;
            doctor.SpecialtyId = model.SpecialtyId;
            doctor.ExperienceYears = model.ExperienceYears;
            doctor.Description = model.Description;
            doctor.ImageUrl = model.ImageUrl;
            doctor.WorkingTime = model.WorkingTime;
            doctor.IsFeatured = model.IsFeatured;
            doctor.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật bác sĩ thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(d => d.Specialty)
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null) return NotFound();

            var model = new AdminDoctorListItemViewModel
            {
                DoctorId = doctor.DoctorId,
                FullName = doctor.FullName,
                SpecialtyName = doctor.Specialty.SpecialtyName,
                ExperienceYears = doctor.ExperienceYears,
                ImageUrl = doctor.ImageUrl,
                IsFeatured = doctor.IsFeatured,
                IsActive = doctor.IsActive
            };

            ViewBag.Description = doctor.Description;
            ViewBag.WorkingTime = doctor.WorkingTime;

            return View(model);
        }
    }
}