using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminDoctorSchedulesController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminDoctorSchedulesController(ClinicBookingContext context)
        {
            _context = context;
        }

        private async Task<IEnumerable<SelectListItem>> GetDoctorSelectListAsync()
        {
            return await _context.Doctors
                .AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.FullName)
                .Select(d => new SelectListItem
                {
                    Value = d.DoctorId.ToString(),
                    Text = d.FullName
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? doctorId, DateOnly? fromDate, DateOnly? toDate)
        {
            var query = _context.DoctorSchedules
                .AsNoTracking()
                .Include(s => s.Doctor)
                .Include(s => s.DoctorScheduleSlots)
                .AsQueryable();

            if (doctorId.HasValue && doctorId > 0)
            {
                query = query.Where(s => s.DoctorId == doctorId);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.WorkDate >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(s => s.WorkDate <= toDate);
            }

            ViewBag.DoctorId = new SelectList(
                await _context.Doctors.AsNoTracking().Where(d => d.IsActive).ToListAsync(),
                "DoctorId", "FullName", doctorId);
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            var model = await query
                .OrderByDescending(s => s.WorkDate)
                .Select(s => new AdminScheduleListItemViewModel
                {
                    ScheduleId = s.ScheduleId,
                    DoctorName = s.Doctor.FullName,
                    WorkDate = s.WorkDate,
                    IsAvailable = s.IsAvailable,
                    SlotCount = s.DoctorScheduleSlots.Count,
                    Notes = s.Notes
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminScheduleCreateViewModel
            {
                WorkDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                IsAvailable = true,
                Doctors = await GetDoctorSelectListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminScheduleCreateViewModel model)
        {
            // Check duplicate
            bool exists = await _context.DoctorSchedules.AnyAsync(
                s => s.DoctorId == model.DoctorId && s.WorkDate == model.WorkDate);
            if (exists)
            {
                ModelState.AddModelError("WorkDate", "Bác sĩ đã có lịch vào ngày này.");
            }

            if (!ModelState.IsValid)
            {
                model.Doctors = await GetDoctorSelectListAsync();
                return View(model);
            }

            var schedule = new DoctorSchedule
            {
                DoctorId = model.DoctorId,
                WorkDate = model.WorkDate,
                Notes = model.Notes?.Trim(),
                IsAvailable = model.IsAvailable,
                CreatedAt = DateTime.Now
            };

            _context.DoctorSchedules.Add(schedule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tạo lịch làm việc thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var schedule = await _context.DoctorSchedules
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null) return NotFound();

            var model = new AdminScheduleEditViewModel
            {
                ScheduleId = schedule.ScheduleId,
                DoctorId = schedule.DoctorId,
                DoctorName = schedule.Doctor.FullName,
                WorkDate = schedule.WorkDate,
                Notes = schedule.Notes,
                IsAvailable = schedule.IsAvailable
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminScheduleEditViewModel model)
        {
            var schedule = await _context.DoctorSchedules.FindAsync(model.ScheduleId);
            if (schedule == null) return NotFound();

            // Check duplicate date for same doctor (excluding current)
            bool exists = await _context.DoctorSchedules.AnyAsync(
                s => s.DoctorId == schedule.DoctorId &&
                     s.WorkDate == model.WorkDate &&
                     s.ScheduleId != model.ScheduleId);
            if (exists)
            {
                ModelState.AddModelError("WorkDate", "Bác sĩ đã có lịch vào ngày này.");
            }

            if (!ModelState.IsValid)
            {
                model.DoctorName = (await _context.Doctors.FindAsync(schedule.DoctorId))?.FullName ?? "";
                return View(model);
            }

            schedule.WorkDate = model.WorkDate;
            schedule.Notes = model.Notes?.Trim();
            schedule.IsAvailable = model.IsAvailable;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật lịch làm việc thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var schedule = await _context.DoctorSchedules
                .Include(s => s.Appointments)
                .Include(s => s.DoctorScheduleSlots)
                .FirstOrDefaultAsync(s => s.ScheduleId == id);

            if (schedule == null) return NotFound();

            if (schedule.Appointments.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa lịch đã có lịch hẹn.";
                return RedirectToAction(nameof(Index));
            }

            // Remove related slots first
            _context.DoctorScheduleSlots.RemoveRange(schedule.DoctorScheduleSlots);
            _context.DoctorSchedules.Remove(schedule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa lịch làm việc thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
