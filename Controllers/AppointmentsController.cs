using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ClinicBookingContext _context;

        private const int PendingStatusId = 1;
        private const int ConfirmedStatusId = 2;
        private const int CompletedStatusId = 3;
        private const int CancelledStatusId = 4;

        public AppointmentsController(ClinicBookingContext context)
        {
            _context = context;
        }

        private async Task LoadDropdowns(AppointmentCreateViewModel model)
        {
            model.Specialties = await _context.Specialties
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.SpecialtyName)
                .Select(s => new SelectListItem
                {
                    Value = s.SpecialtyId.ToString(),
                    Text = s.SpecialtyName
                })
                .ToListAsync();

            if (model.SpecialtyId > 0)
            {
                model.Doctors = await _context.Doctors
                    .AsNoTracking()
                    .Where(d => d.IsActive && d.SpecialtyId == model.SpecialtyId)
                    .OrderBy(d => d.FullName)
                    .Select(d => new SelectListItem
                    {
                        Value = d.DoctorId.ToString(),
                        Text = d.FullName
                    })
                    .ToListAsync();
            }
            else
            {
                model.Doctors = new List<SelectListItem>();
            }

            if (model.DoctorId > 0 && model.AppointmentDate != default)
            {
                model.ScheduleSlots = await _context.DoctorScheduleSlots
                    .AsNoTracking()
                    .Include(s => s.Schedule)
                    .Include(s => s.TimeSlot)
                    .Where(s =>
                        s.IsAvailable &&
                        s.CurrentAppointments < s.MaxAppointments &&
                        s.Schedule.IsAvailable &&
                        s.Schedule.DoctorId == model.DoctorId &&
                        s.Schedule.WorkDate == model.AppointmentDate)
                    .OrderBy(s => s.TimeSlot.StartTime)
                    .Select(s => new SelectListItem
                    {
                        Value = s.DoctorScheduleSlotId.ToString(),
                        Text = s.TimeSlot.SlotLabel ?? $"{s.TimeSlot.StartTime:HH\\:mm} - {s.TimeSlot.EndTime:HH\\:mm}"
                    })
                    .ToListAsync();
            }
            else
            {
                model.ScheduleSlots = new List<SelectListItem>();
            }
        }

        private async Task PrefillPatientInfoAsync(AppointmentCreateViewModel model, int userId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (user != null)
            {
                if (string.IsNullOrWhiteSpace(model.PatientName))
                    model.PatientName = user.FullName;

                if (string.IsNullOrWhiteSpace(model.Email))
                    model.Email = user.Email;

                if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                    model.PhoneNumber = user.PhoneNumber;
            }

            if (patient != null)
            {
                model.DateOfBirth ??= patient.DateOfBirth;
                model.Gender ??= patient.Gender;
            }
        }

        private static string GenerateBookingCode()
        {
            return $"BK{DateTime.Now:yyyyMMddHHmmssfff}";
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AppointmentCreateViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1))
            };

            await PrefillPatientInfoAsync(model, userId.Value);
            await LoadDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            model.PatientName = model.PatientName?.Trim() ?? string.Empty;
            model.PhoneNumber = model.PhoneNumber?.Trim() ?? string.Empty;
            model.Email = model.Email?.Trim() ?? string.Empty;
            model.Gender = string.IsNullOrWhiteSpace(model.Gender) ? null : model.Gender.Trim();
            model.Symptoms = string.IsNullOrWhiteSpace(model.Symptoms) ? null : model.Symptoms.Trim();

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (model.AppointmentDate < today)
            {
                ModelState.AddModelError("AppointmentDate", "Ngày khám phải từ hôm nay trở đi.");
            }

            if (model.DateOfBirth.HasValue)
            {
                if (model.DateOfBirth.Value > today)
                {
                    ModelState.AddModelError("DateOfBirth", "Ngày sinh không được lớn hơn ngày hiện tại.");
                }

                if (model.DateOfBirth.Value.Year < 1900)
                {
                    ModelState.AddModelError("DateOfBirth", "Ngày sinh không hợp lệ.");
                }
            }

            var slot = await _context.DoctorScheduleSlots
                .Include(s => s.Schedule)
                    .ThenInclude(sc => sc.Doctor)
                .Include(s => s.TimeSlot)
                .FirstOrDefaultAsync(s =>
                    s.DoctorScheduleSlotId == model.DoctorScheduleSlotId &&
                    s.IsAvailable);

            if (slot == null)
            {
                ModelState.AddModelError("DoctorScheduleSlotId", "Khung giờ không hợp lệ.");
            }
            else
            {
                if (!slot.Schedule.IsAvailable)
                {
                    ModelState.AddModelError("DoctorScheduleSlotId", "Lịch làm việc này hiện không khả dụng.");
                }

                if (slot.CurrentAppointments >= slot.MaxAppointments)
                {
                    ModelState.AddModelError("DoctorScheduleSlotId", "Khung giờ đã đầy.");
                }

                if (slot.Schedule.DoctorId != model.DoctorId)
                {
                    ModelState.AddModelError("DoctorId", "Bác sĩ được chọn không khớp với khung giờ.");
                }

                if (slot.Schedule.Doctor.SpecialtyId != model.SpecialtyId)
                {
                    ModelState.AddModelError("SpecialtyId", "Chuyên khoa được chọn không khớp với bác sĩ.");
                }

                if (slot.Schedule.WorkDate != model.AppointmentDate)
                {
                    ModelState.AddModelError("AppointmentDate", "Ngày khám không khớp với khung giờ đã chọn.");
                }

                bool duplicateAppointment = await _context.Appointments.AnyAsync(a =>
                    a.UserPatientId == userId.Value &&
                    a.AppointmentDate == slot.Schedule.WorkDate &&
                    a.TimeSlotId == slot.TimeSlotId &&
                    a.StatusId != CancelledStatusId);

                if (duplicateAppointment)
                {
                    ModelState.AddModelError(string.Empty, "Bạn đã có một lịch hẹn khác trong đúng khung giờ này.");
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var appointment = new Appointment
                {
                    UserPatientId = userId.Value,
                    PatientName = model.PatientName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,

                    DoctorId = model.DoctorId,
                    SpecialtyId = model.SpecialtyId,

                    ScheduleId = slot!.ScheduleId,
                    TimeSlotId = slot.TimeSlotId,
                    DoctorScheduleSlotId = slot.DoctorScheduleSlotId,

                    AppointmentDate = slot.Schedule.WorkDate,
                    AppointmentTime = slot.TimeSlot.StartTime,

                    Symptoms = model.Symptoms,
                    StatusId = PendingStatusId,
                    BookingCode = GenerateBookingCode(),
                    CreatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);

                slot.CurrentAppointments += 1;

                await _context.SaveChangesAsync();

                var history = new AppointmentStatusHistory
                {
                    AppointmentId = appointment.AppointmentId,
                    OldStatusId = null,
                    NewStatusId = PendingStatusId,
                    ChangedByUserId = userId.Value,
                    ChangedAt = DateTime.Now,
                    Note = "Tạo lịch hẹn"
                };

                _context.AppointmentStatusHistories.Add(history);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đặt lịch thành công.";
                return RedirectToAction(nameof(MyAppointments));
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Đặt lịch thất bại. Vui lòng thử lại.");
                await LoadDropdowns(model);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctorsBySpecialty(int specialtyId)
        {
            var doctors = await _context.Doctors
                .AsNoTracking()
                .Where(d => d.IsActive && d.SpecialtyId == specialtyId)
                .OrderBy(d => d.FullName)
                .Select(d => new
                {
                    value = d.DoctorId,
                    text = d.FullName
                })
                .ToListAsync();

            return Json(doctors);
        }

        [HttpGet]
        public async Task<IActionResult> GetSlots(int doctorId, DateOnly appointmentDate)
        {
            var slots = await _context.DoctorScheduleSlots
                .AsNoTracking()
                .Include(s => s.Schedule)
                .Include(s => s.TimeSlot)
                .Where(s =>
                    s.IsAvailable &&
                    s.CurrentAppointments < s.MaxAppointments &&
                    s.Schedule.IsAvailable &&
                    s.Schedule.DoctorId == doctorId &&
                    s.Schedule.WorkDate == appointmentDate)
                .OrderBy(s => s.TimeSlot.StartTime)
                .Select(s => new
                {
                    value = s.DoctorScheduleSlotId,
                    text = s.TimeSlot.SlotLabel ?? $"{s.TimeSlot.StartTime:HH\\:mm} - {s.TimeSlot.EndTime:HH\\:mm}"
                })
                .ToListAsync();

            return Json(slots);
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .Where(a => a.UserPatientId == userId.Value)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Select(a => new AppointmentListItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.PatientName,
                    DoctorName = a.Doctor.FullName,
                    SpecialtyName = a.Specialty.SpecialtyName,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    StatusName = a.Status.StatusName,
                    BookingCode = a.BookingCode
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.UserPatientId == userId.Value);

            if (appointment == null)
            {
                return NotFound();
            }

            var model = new AppointmentDetailsViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PatientName = appointment.PatientName,
                PhoneNumber = appointment.PhoneNumber,
                Email = appointment.Email,
                DoctorName = appointment.Doctor.FullName,
                SpecialtyName = appointment.Specialty.SpecialtyName,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Symptoms = appointment.Symptoms,
                StatusName = appointment.Status.StatusName,
                BookingCode = appointment.BookingCode,
                CreatedAt = appointment.CreatedAt
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int appointmentId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments
                .Include(a => a.DoctorScheduleSlot)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.UserPatientId == userId.Value);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn.";
                return RedirectToAction(nameof(MyAppointments));
            }

            if (appointment.StatusId == CancelledStatusId)
            {
                TempData["ErrorMessage"] = "Lịch hẹn này đã bị hủy trước đó.";
                return RedirectToAction(nameof(MyAppointments));
            }

            if (appointment.StatusId == CompletedStatusId)
            {
                TempData["ErrorMessage"] = "Không thể hủy lịch hẹn đã hoàn thành.";
                return RedirectToAction(nameof(MyAppointments));
            }

            if (appointment.AppointmentDate <= DateOnly.FromDateTime(DateTime.Today))
            {
                TempData["ErrorMessage"] = "Bạn chỉ có thể hủy lịch trước ngày khám.";
                return RedirectToAction(nameof(MyAppointments));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int oldStatusId = appointment.StatusId;

                appointment.StatusId = CancelledStatusId;
                appointment.UpdatedAt = DateTime.Now;

                if (appointment.DoctorScheduleSlot != null && appointment.DoctorScheduleSlot.CurrentAppointments > 0)
                {
                    appointment.DoctorScheduleSlot.CurrentAppointments -= 1;
                }

                await _context.SaveChangesAsync();

                var history = new AppointmentStatusHistory
                {
                    AppointmentId = appointment.AppointmentId,
                    OldStatusId = oldStatusId,
                    NewStatusId = CancelledStatusId,
                    ChangedByUserId = userId.Value,
                    ChangedAt = DateTime.Now,
                    Note = "Bệnh nhân hủy lịch"
                };

                _context.AppointmentStatusHistories.Add(history);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Hủy lịch hẹn thành công.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "Hủy lịch thất bại. Vui lòng thử lại.";
            }

            return RedirectToAction(nameof(MyAppointments));
        }
    }
}