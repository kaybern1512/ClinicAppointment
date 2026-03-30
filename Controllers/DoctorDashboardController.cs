using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Doctor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ClinicBookingMVC.Controllers
{
    public class DoctorDashboardController : Controller
    {
        private readonly ClinicBookingContext _context;
        private readonly ILogger<DoctorDashboardController> _logger;

        public DoctorDashboardController(ClinicBookingContext context, ILogger<DoctorDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool IsDoctor() => HttpContext.Session.GetString("RoleName") == "Doctor";
        private string GetUserFullName() => HttpContext.Session.GetString("UserFullName") ?? "";

        public async Task<IActionResult> Index()
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Account");

            _logger.LogInformation("Doctor Dashboard accessed.");

            try
            {
                var userFullName = GetUserFullName();
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == userFullName);

                if (doctor == null)
                {
                    _logger.LogWarning("User {FullName} does not have an associated Doctor entity.", userFullName);
                    ViewData["ErrorMessage"] = "Không tìm thấy thông tin bác sĩ tương ứng với tên của bạn.";
                    return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

                var today = DateOnly.FromDateTime(DateTime.Today);
                var appointmentsQuery = _context.Appointments.Where(a => a.DoctorId == doctor.DoctorId);

                var viewModel = new DoctorDashboardViewModel
                {
                    TodayAppointments = await appointmentsQuery.CountAsync(a => a.AppointmentDate == today),
                    PendingAppointments = await appointmentsQuery.CountAsync(a => a.Status.StatusName == "Pending"),
                    ConfirmedAppointments = await appointmentsQuery.CountAsync(a => a.Status.StatusName == "Confirmed"),
                    TotalAppointments = await appointmentsQuery.CountAsync(),

                    UpcomingAppointments = await appointmentsQuery
                        .Include(a => a.Status)
                        .Where(a => a.AppointmentDate >= today && a.Status.StatusName != "Completed" && a.Status.StatusName != "Cancelled")
                        .OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime)
                        .Take(10)
                        .Select(a => new ViewModels.AppointmentListItemViewModel
                        {
                            AppointmentId = a.AppointmentId,
                            PatientName = a.PatientName,
                            PatientPhoneNumber = a.PhoneNumber,
                            AppointmentDate = a.AppointmentDate,
                            AppointmentTime = a.AppointmentTime,
                            StatusName = a.Status.StatusName
                        })
                        .ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data for the Doctor Dashboard.");
                ViewData["ErrorMessage"] = "Đã có lỗi xảy ra khi tải dữ liệu cho bảng điều khiển.";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> AppointmentDetails(int id)
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Account");

            var userFullName = GetUserFullName();
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == userFullName);
            if (doctor == null) return RedirectToAction("Index");

            var appointment = await _context.Appointments
                .Include(a => a.UserPatient)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.DoctorId == doctor.DoctorId);

            if (appointment == null) return NotFound();

            var record = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.AppointmentId == id);
            ViewBag.ExistingRecord = record;

            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            bool canWriteMedicalRecord = true;
            string? medicalRecordBlockedReason = null;

            if (appointment.Status?.StatusName == "Cancelled")
            {
                canWriteMedicalRecord = false;
                medicalRecordBlockedReason = "Lịch hẹn đã bị hủy nên không thể tạo hồ sơ bệnh án.";
            }
            else if (record != null)
            {
                canWriteMedicalRecord = false;
                medicalRecordBlockedReason = "Lịch hẹn này đã có hồ sơ bệnh án.";
            }
            else if (appointment.AppointmentDate > today)
            {
                canWriteMedicalRecord = false;
                medicalRecordBlockedReason = "Chưa tới ngày khám.";
            }
            else if (appointment.AppointmentDate == today && appointment.AppointmentTime > currentTime)
            {
                canWriteMedicalRecord = false;
                medicalRecordBlockedReason = "Chưa tới giờ khám.";
            }

            ViewBag.CanWriteMedicalRecord = canWriteMedicalRecord;
            ViewBag.MedicalRecordBlockedReason = medicalRecordBlockedReason;

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMedicalRecord(int appointmentId, string diagnosis, string prescription, string doctorNotes)
        {
            if (!IsDoctor())
                return RedirectToAction("Login", "Account");

            var userFullName = GetUserFullName();
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == userFullName);
            if (doctor == null)
                return RedirectToAction("Index");

            var appointment = await _context.Appointments
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.DoctorId == doctor.DoctorId);

            if (appointment == null)
                return NotFound();

            diagnosis = diagnosis?.Trim() ?? string.Empty;
            prescription = prescription?.Trim() ?? string.Empty;
            doctorNotes = doctorNotes?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(diagnosis))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập chẩn đoán trước khi lưu bệnh án.";
                return RedirectToAction("AppointmentDetails", new { id = appointmentId });
            }

            if (appointment.Status?.StatusName == "Cancelled")
            {
                TempData["ErrorMessage"] = "Không thể tạo bệnh án cho lịch hẹn đã bị hủy.";
                return RedirectToAction("AppointmentDetails", new { id = appointmentId });
            }

            var today = DateOnly.FromDateTime(DateTime.Now);
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            if (appointment.AppointmentDate > today)
            {
                TempData["ErrorMessage"] = "Chưa tới ngày khám, không thể tạo hồ sơ bệnh án.";
                return RedirectToAction("AppointmentDetails", new { id = appointmentId });
            }

            if (appointment.AppointmentDate == today && appointment.AppointmentTime > currentTime)
            {
                TempData["ErrorMessage"] = "Chưa tới giờ khám, không thể tạo hồ sơ bệnh án.";
                return RedirectToAction("AppointmentDetails", new { id = appointmentId });
            }

            var existingRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.AppointmentId == appointmentId);

            if (existingRecord != null)
            {
                TempData["ErrorMessage"] = "Lịch hẹn này đã có hồ sơ bệnh án rồi.";
                return RedirectToAction("AppointmentDetails", new { id = appointmentId });
            }

            var newRecord = new MedicalRecord
            {
                AppointmentId = appointmentId,
                Diagnosis = diagnosis,
                Prescription = string.IsNullOrWhiteSpace(prescription) ? null : prescription,
                DoctorNotes = string.IsNullOrWhiteSpace(doctorNotes) ? null : doctorNotes,
                CreatedAt = DateTime.Now
            };

            _context.MedicalRecords.Add(newRecord);

            var completedStatus = await _context.AppointmentStatuses
                .FirstOrDefaultAsync(s => s.StatusName == "Completed");

            if (completedStatus != null && appointment.StatusId != completedStatus.StatusId)
            {
                appointment.StatusId = completedStatus.StatusId;
                appointment.UpdatedAt = DateTime.Now;
                _context.Appointments.Update(appointment);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã lưu bệnh án và hoàn thành lịch hẹn thành công.";
            return RedirectToAction("AppointmentDetails", new { id = appointmentId });
        }

        [HttpGet]
        public async Task<IActionResult> MySchedules(DateOnly? fromDate)
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Account");

            var userFullName = GetUserFullName();
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == userFullName);
            if (doctor == null) return RedirectToAction("Index");

            var query = _context.DoctorSchedules
                .AsNoTracking()
                .Where(s => s.DoctorId == doctor.DoctorId)
                .Include(s => s.DoctorScheduleSlots)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.WorkDate >= fromDate);
            }
            else
            {
                query = query.Where(s => s.WorkDate >= DateOnly.FromDateTime(DateTime.Today));
            }

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");

            var schedules = await query
                .OrderBy(s => s.WorkDate)
                .ToListAsync();

            return View(schedules);
        }

        [HttpGet]
        public async Task<IActionResult> ScheduleDetails(int id)
        {
            if (!IsDoctor()) return RedirectToAction("Login", "Account");

            var userFullName = GetUserFullName();
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == userFullName);
            if (doctor == null) return RedirectToAction("Index");

            var schedule = await _context.DoctorSchedules
                .AsNoTracking()
                .Include(s => s.DoctorScheduleSlots)
                    .ThenInclude(slot => slot.TimeSlot)
                .Include(s => s.DoctorScheduleSlots)
                    .ThenInclude(slot => slot.Appointments)
                        .ThenInclude(a => a.Status)
                .FirstOrDefaultAsync(s => s.ScheduleId == id && s.DoctorId == doctor.DoctorId);

            if (schedule == null) return NotFound();

            return View(schedule);
        }
    }
}
