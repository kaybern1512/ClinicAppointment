using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminDoctorScheduleSlotsController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminDoctorScheduleSlotsController(ClinicBookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int scheduleId)
        {
            var schedule = await _context.DoctorSchedules
                .AsNoTracking()
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null) return NotFound();

            ViewBag.ScheduleId = scheduleId;
            ViewBag.DoctorName = schedule.Doctor.FullName;
            ViewBag.WorkDate = schedule.WorkDate;

            var model = await _context.DoctorScheduleSlots
                .AsNoTracking()
                .Where(s => s.ScheduleId == scheduleId)
                .Include(s => s.TimeSlot)
                .OrderBy(s => s.TimeSlot.StartTime)
                .Select(s => new AdminSlotListItemViewModel
                {
                    DoctorScheduleSlotId = s.DoctorScheduleSlotId,
                    TimeSlotLabel = s.TimeSlot.SlotLabel ?? $"{s.TimeSlot.StartTime:HH\\:mm} - {s.TimeSlot.EndTime:HH\\:mm}",
                    StartTime = s.TimeSlot.StartTime,
                    EndTime = s.TimeSlot.EndTime,
                    MaxAppointments = s.MaxAppointments,
                    CurrentAppointments = s.CurrentAppointments,
                    IsAvailable = s.IsAvailable
                })
                .ToListAsync();

            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetAvailableTimeSlotsAsync(int scheduleId)
        {
            var usedSlotIds = await _context.DoctorScheduleSlots
                .Where(s => s.ScheduleId == scheduleId)
                .Select(s => s.TimeSlotId)
                .ToListAsync();

            return await _context.TimeSlots
                .AsNoTracking()
                .Where(t => t.IsActive && !usedSlotIds.Contains(t.TimeSlotId))
                .OrderBy(t => t.StartTime)
                .Select(t => new SelectListItem
                {
                    Value = t.TimeSlotId.ToString(),
                    Text = t.SlotLabel ?? $"{t.StartTime:HH\\:mm} - {t.EndTime:HH\\:mm}"
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Create(int scheduleId)
        {
            var schedule = await _context.DoctorSchedules
                .AsNoTracking()
                .Include(s => s.Doctor)
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);

            if (schedule == null) return NotFound();

            var model = new AdminSlotCreateViewModel
            {
                ScheduleId = scheduleId,
                MaxAppointments = 1,
                DoctorName = schedule.Doctor.FullName,
                WorkDate = schedule.WorkDate,
                TimeSlots = await GetAvailableTimeSlotsAsync(scheduleId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminSlotCreateViewModel model)
        {
            // Check duplicate
            bool exists = await _context.DoctorScheduleSlots.AnyAsync(
                s => s.ScheduleId == model.ScheduleId && s.TimeSlotId == model.TimeSlotId);
            if (exists)
            {
                ModelState.AddModelError("TimeSlotId", "Khung giờ này đã tồn tại trong lịch.");
            }

            if (!ModelState.IsValid)
            {
                var schedule = await _context.DoctorSchedules
                    .AsNoTracking().Include(s => s.Doctor)
                    .FirstOrDefaultAsync(s => s.ScheduleId == model.ScheduleId);
                model.DoctorName = schedule?.Doctor.FullName ?? "";
                model.WorkDate = schedule?.WorkDate ?? DateOnly.MinValue;
                model.TimeSlots = await GetAvailableTimeSlotsAsync(model.ScheduleId);
                return View(model);
            }

            var slot = new DoctorScheduleSlot
            {
                ScheduleId = model.ScheduleId,
                TimeSlotId = model.TimeSlotId,
                MaxAppointments = model.MaxAppointments,
                CurrentAppointments = 0,
                IsAvailable = true
            };

            _context.DoctorScheduleSlots.Add(slot);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm slot thành công.";
            return RedirectToAction(nameof(Index), new { scheduleId = model.ScheduleId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var slot = await _context.DoctorScheduleSlots
                .Include(s => s.TimeSlot)
                .FirstOrDefaultAsync(s => s.DoctorScheduleSlotId == id);

            if (slot == null) return NotFound();

            var model = new AdminSlotEditViewModel
            {
                DoctorScheduleSlotId = slot.DoctorScheduleSlotId,
                ScheduleId = slot.ScheduleId,
                TimeSlotLabel = slot.TimeSlot.SlotLabel ?? $"{slot.TimeSlot.StartTime:HH\\:mm} - {slot.TimeSlot.EndTime:HH\\:mm}",
                StartTime = slot.TimeSlot.StartTime,
                EndTime = slot.TimeSlot.EndTime,
                MaxAppointments = slot.MaxAppointments,
                CurrentAppointments = slot.CurrentAppointments,
                IsAvailable = slot.IsAvailable
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminSlotEditViewModel model)
        {
            var slot = await _context.DoctorScheduleSlots.FindAsync(model.DoctorScheduleSlotId);
            if (slot == null) return NotFound();

            if (model.MaxAppointments < slot.CurrentAppointments)
            {
                ModelState.AddModelError("MaxAppointments",
                    $"Số lượng tối đa không thể nhỏ hơn số đã đặt ({slot.CurrentAppointments}).");
            }

            if (!ModelState.IsValid)
            {
                var timeSlot = await _context.TimeSlots.FindAsync(slot.TimeSlotId);
                model.TimeSlotLabel = timeSlot?.SlotLabel ?? "";
                model.StartTime = timeSlot?.StartTime ?? TimeOnly.MinValue;
                model.EndTime = timeSlot?.EndTime ?? TimeOnly.MinValue;
                model.CurrentAppointments = slot.CurrentAppointments;
                model.ScheduleId = slot.ScheduleId;
                return View(model);
            }

            slot.MaxAppointments = model.MaxAppointments;
            slot.IsAvailable = model.IsAvailable;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật slot thành công.";
            return RedirectToAction(nameof(Index), new { scheduleId = slot.ScheduleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var slot = await _context.DoctorScheduleSlots.FindAsync(id);
            if (slot == null) return NotFound();

            if (slot.CurrentAppointments > 0)
            {
                TempData["ErrorMessage"] = "Không thể xóa slot đã có lịch hẹn.";
                return RedirectToAction(nameof(Index), new { scheduleId = slot.ScheduleId });
            }

            var scheduleId = slot.ScheduleId;
            _context.DoctorScheduleSlots.Remove(slot);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa slot thành công.";
            return RedirectToAction(nameof(Index), new { scheduleId });
        }
    }
}
