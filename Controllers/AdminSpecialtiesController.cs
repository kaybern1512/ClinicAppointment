using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminSpecialtiesController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminSpecialtiesController(ClinicBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Specialties
                .OrderBy(s => s.SpecialtyId)
                .Select(s => new AdminSpecialtyListItemViewModel
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

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminSpecialtyCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminSpecialtyCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var specialty = new Specialty
            {
                SpecialtyName = model.SpecialtyName,
                Description = model.Description,
                Icon = model.Icon,
                ImageUrl = model.ImageUrl,
                IsFeatured = model.IsFeatured
            };

            _context.Specialties.Add(specialty);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm chuyên khoa thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null) return NotFound();

            var model = new AdminSpecialtyEditViewModel
            {
                SpecialtyId = specialty.SpecialtyId,
                SpecialtyName = specialty.SpecialtyName,
                Description = specialty.Description,
                Icon = specialty.Icon,
                ImageUrl = specialty.ImageUrl,
                IsFeatured = specialty.IsFeatured
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminSpecialtyEditViewModel model)
        {
            var specialty = await _context.Specialties.FindAsync(model.SpecialtyId);
            if (specialty == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            specialty.SpecialtyName = model.SpecialtyName;
            specialty.Description = model.Description;
            specialty.Icon = model.Icon;
            specialty.ImageUrl = model.ImageUrl;
            specialty.IsFeatured = model.IsFeatured;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật chuyên khoa thành công.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var specialty = await _context.Specialties
                .FirstOrDefaultAsync(s => s.SpecialtyId == id);

            if (specialty == null) return NotFound();

            var model = new AdminSpecialtyListItemViewModel
            {
                SpecialtyId = specialty.SpecialtyId,
                SpecialtyName = specialty.SpecialtyName,
                Description = specialty.Description,
                Icon = specialty.Icon,
                ImageUrl = specialty.ImageUrl,
                IsFeatured = specialty.IsFeatured
            };

            return View(model);
        }
    }
}