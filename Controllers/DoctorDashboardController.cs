using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace ClinicBookingMVC.Controllers
{
    [Authorize(Roles = "Doctor")]
    public class DoctorDashboardController : Controller
    {
        private readonly ClinicBookingContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DoctorDashboardController> _logger;

        public DoctorDashboardController(ClinicBookingContext context, UserManager<User> userManager, ILogger<DoctorDashboardController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Doctor Dashboard accessed by user {User}", User.Identity.Name);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Current user not found for {User}", User.Identity.Name);
                    return RedirectToAction("Login", "Account");
                }

                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == user.FullName);

                if (doctor == null)
                {
                    _logger.LogWarning("User {FullName} does not have an associated Doctor entity.", user.FullName);
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
                        .Select(a => new ViewModels.AppointmentListItemViewModel // Specify namespace to avoid ambiguity
                        {
                            AppointmentId = a.AppointmentId,
                            PatientName = a.PatientName,
                            PatientPhoneNumber = a.PhoneNumber, // Corrected property name
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
                _logger.LogError(ex, "An error occurred while fetching data for the Doctor Dashboard for user {User}", User.Identity.Name);
                ViewData["ErrorMessage"] = "Đã có lỗi xảy ra khi tải dữ liệu cho bảng điều khiển.";
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> AppointmentDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == user.FullName);
            if (doctor == null) return RedirectToAction("Index");

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.DoctorId == doctor.DoctorId);

            if (appointment == null) return NotFound();

            var record = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            ViewBag.ExistingRecord = record;

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveMedicalRecord(int appointmentId, string diagnosis, string prescription, string doctorNotes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index");

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.FullName == user.FullName);
            if (doctor == null) return RedirectToAction("Index");

            var appointment = await _context.Appointments
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.DoctorId == doctor.DoctorId);

            if (appointment == null) return NotFound();

            // Check if record already exists
            var existingRecord = await _context.MedicalRecords.FirstOrDefaultAsync(m => m.AppointmentId == appointmentId);
            if (existingRecord == null)
            {
                var newRecord = new MedicalRecord
                {
                    AppointmentId = appointmentId,
                    Diagnosis = diagnosis,
                    Prescription = prescription,
                    DoctorNotes = doctorNotes,
                    CreatedAt = DateTime.Now
                };
                _context.MedicalRecords.Add(newRecord);

                // Update Appointment Status to "Completed" (Find status ID first)
                var completedStatus = await _context.AppointmentStatuses.FirstOrDefaultAsync(s => s.StatusName == "Completed");
                if (completedStatus != null)
                {
                    appointment.StatusId = completedStatus.StatusId;
                    _context.Appointments.Update(appointment);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã lưu bệnh án và hoàn thành lịch hẹn thành công.";
            }

            return RedirectToAction("AppointmentDetails", new { id = appointmentId });
        }
    }
}
