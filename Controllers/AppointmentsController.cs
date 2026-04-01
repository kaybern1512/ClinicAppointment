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
                var now = DateTime.Now;
                var today = DateOnly.FromDateTime(now);
                var maxBookingDate = today.AddDays(7);

                if (model.AppointmentDate >= today && model.AppointmentDate <= maxBookingDate)
                {
                    var rawSlots = await _context.DoctorScheduleSlots
                        .AsNoTracking()
                        .Where(s =>
                            s.IsAvailable &&
                            s.CurrentAppointments < s.MaxAppointments &&
                            s.Schedule.IsAvailable &&
                            s.Schedule.DoctorId == model.DoctorId &&
                            s.Schedule.WorkDate == model.AppointmentDate &&
                            s.TimeSlot.IsActive)
                        .OrderBy(s => s.TimeSlot.StartTime)
                        .Select(s => new
                        {
                            s.DoctorScheduleSlotId,
                            s.Schedule.WorkDate,
                            s.TimeSlot.StartTime,
                            s.TimeSlot.EndTime,
                            s.TimeSlot.SlotLabel
                        })
                        .ToListAsync();

                    model.ScheduleSlots = rawSlots
                        .Where(s =>
                        {
                            var appointmentDateTime = new DateTime(
                                s.WorkDate.Year,
                                s.WorkDate.Month,
                                s.WorkDate.Day,
                                s.StartTime.Hour,
                                s.StartTime.Minute,
                                0
                            );

                            return appointmentDateTime > now;
                        })
                        .Select(s => new SelectListItem
                        {
                            Value = s.DoctorScheduleSlotId.ToString(),
                            Text = !string.IsNullOrEmpty(s.SlotLabel)
                                ? s.SlotLabel
                                : s.StartTime.ToString(@"HH\:mm") + " - " + s.EndTime.ToString(@"HH\:mm")
                        })
                        .ToList();
                }
                else
                {
                    model.ScheduleSlots = new List<SelectListItem>();
                }
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

        private void SetToastErrorsFromModelState()
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Distinct()
                .ToList();

            ViewBag.ToastErrors = errors;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt lịch.";
                return RedirectToAction("Login", "Account");
            }

            var model = new AppointmentCreateViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today)
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
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt lịch.";
                return RedirectToAction("Login", "Account");
            }

            model.PatientName = model.PatientName?.Trim() ?? string.Empty;
            model.PhoneNumber = model.PhoneNumber?.Trim() ?? string.Empty;
            model.Email = model.Email?.Trim() ?? string.Empty;
            model.Gender = string.IsNullOrWhiteSpace(model.Gender) ? null : model.Gender.Trim();
            model.Symptoms = string.IsNullOrWhiteSpace(model.Symptoms) ? null : model.Symptoms.Trim();

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var maxBookingDate = today.AddDays(7);

            if (model.AppointmentDate < today)
            {
                ModelState.AddModelError("AppointmentDate", "Không thể đặt lịch trong quá khứ.");
            }

            if (model.AppointmentDate > maxBookingDate)
            {
                ModelState.AddModelError("AppointmentDate", "Chỉ được đặt lịch tối đa trong vòng 1 tuần tới.");
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
                    s.IsAvailable &&
                    s.TimeSlot.IsActive);

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

                if (slot.Schedule.WorkDate > maxBookingDate)
                {
                    ModelState.AddModelError("AppointmentDate", "Chỉ được đặt lịch tối đa trong vòng 1 tuần tới.");
                }

                var appointmentDateTime = new DateTime(
                    slot.Schedule.WorkDate.Year,
                    slot.Schedule.WorkDate.Month,
                    slot.Schedule.WorkDate.Day,
                    slot.TimeSlot.StartTime.Hour,
                    slot.TimeSlot.StartTime.Minute,
                    0
                );

                if (appointmentDateTime <= now)
                {
                    ModelState.AddModelError("DoctorScheduleSlotId", "Không thể đặt lịch ở khung giờ đã qua.");
                }

                var appointmentsInSameDay = await _context.Appointments.CountAsync(a =>
                    a.UserPatientId == userId.Value &&
                    a.AppointmentDate == slot.Schedule.WorkDate &&
                    a.StatusId != CancelledStatusId);

                if (appointmentsInSameDay >= 2)
                {
                    ModelState.AddModelError(string.Empty, "Bạn chỉ được đặt tối đa 2 lịch trong 1 ngày.");
                }

                bool duplicateTime = await _context.Appointments.AnyAsync(a =>
                    a.UserPatientId == userId.Value &&
                    a.AppointmentDate == slot.Schedule.WorkDate &&
                    a.TimeSlotId == slot.TimeSlotId &&
                    a.StatusId != CancelledStatusId);

                if (duplicateTime)
                {
                    ModelState.AddModelError(string.Empty, "Bạn đã có lịch hẹn khác trong cùng khung giờ này.");
                }

                bool duplicateDoctorAndTime = await _context.Appointments.AnyAsync(a =>
                    a.UserPatientId == userId.Value &&
                    a.DoctorId == slot.Schedule.DoctorId &&
                    a.AppointmentDate == slot.Schedule.WorkDate &&
                    a.TimeSlotId == slot.TimeSlotId &&
                    a.StatusId != CancelledStatusId);

                if (duplicateDoctorAndTime)
                {
                    ModelState.AddModelError(string.Empty, "Bạn đã đặt lịch với bác sĩ này trong khung giờ này.");
                }
            }

            if (!ModelState.IsValid)
            {
                SetToastErrorsFromModelState();
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

                return RedirectToAction("CreatePaymentUrl", "Payment", new { appointmentId = appointment.AppointmentId });
            }
            catch
            {
                await transaction.RollbackAsync();

                ViewBag.ToastErrors = new List<string>
                {
                    "Đặt lịch thất bại. Vui lòng thử lại."
                };

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
        public async Task<IActionResult> GetSlots(int doctorId, string appointmentDate)
        {
            if (doctorId <= 0 || string.IsNullOrWhiteSpace(appointmentDate))
            {
                return Json(new List<object>());
            }

            if (!DateOnly.TryParse(appointmentDate, out var workDate))
            {
                return Json(new List<object>());
            }

            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            var maxBookingDate = today.AddDays(7);

            if (workDate < today || workDate > maxBookingDate)
            {
                return Json(new List<object>());
            }

            var rawSlots = await _context.DoctorScheduleSlots
                .AsNoTracking()
                .Where(s =>
                    s.IsAvailable &&
                    s.CurrentAppointments < s.MaxAppointments &&
                    s.Schedule.IsAvailable &&
                    s.Schedule.DoctorId == doctorId &&
                    s.Schedule.WorkDate == workDate &&
                    s.TimeSlot.IsActive)
                .OrderBy(s => s.TimeSlot.StartTime)
                .Select(s => new
                {
                    s.DoctorScheduleSlotId,
                    s.Schedule.WorkDate,
                    s.TimeSlot.StartTime,
                    s.TimeSlot.EndTime,
                    s.TimeSlot.SlotLabel
                })
                .ToListAsync();

            var slots = rawSlots
                .Where(s =>
                {
                    var appointmentDateTime = new DateTime(
                        s.WorkDate.Year,
                        s.WorkDate.Month,
                        s.WorkDate.Day,
                        s.StartTime.Hour,
                        s.StartTime.Minute,
                        0
                    );

                    return appointmentDateTime > now;
                })
                .Select(s => new
                {
                    value = s.DoctorScheduleSlotId,
                    text = !string.IsNullOrEmpty(s.SlotLabel)
                        ? s.SlotLabel
                        : s.StartTime.ToString(@"HH\:mm") + " - " + s.EndTime.ToString(@"HH\:mm")
                })
                .ToList();

            return Json(slots);
        }

        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem lịch hẹn.";
                return RedirectToAction("Login", "Account");
            }

            var model = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .Include(a => a.Payments)
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
                    BookingCode = a.BookingCode,
                    IsDepositPaid = a.Payments.Any(p => p.PaymentStatus == "Success")
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
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem chi tiết lịch hẹn.";
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .Include(a => a.Payments)
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
                CreatedAt = appointment.CreatedAt,
                IsDepositPaid = appointment.Payments.Any(p => p.PaymentStatus == "Success")
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
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để hủy lịch.";
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

            var today = DateOnly.FromDateTime(DateTime.Today);
            if (appointment.AppointmentDate < today)
            {
                TempData["ErrorMessage"] = "Không thể hủy lịch hẹn trong quá khứ.";
                return RedirectToAction(nameof(MyAppointments));
            }

            var now = DateTime.Now;
            var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
            if (now.DayOfWeek == DayOfWeek.Sunday)
            {
                startOfWeek = now.Date.AddDays(-6);
            }

            var endOfWeek = startOfWeek.AddDays(7);

            var cancelCountThisWeek = await _context.AppointmentStatusHistories.CountAsync(h =>
                h.ChangedByUserId == userId.Value &&
                h.NewStatusId == CancelledStatusId &&
                h.ChangedAt >= startOfWeek &&
                h.ChangedAt < endOfWeek);

            if (cancelCountThisWeek >= 2)
            {
                TempData["ErrorMessage"] = "Bạn chỉ được hủy tối đa 2 lịch trong 1 tuần.";
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