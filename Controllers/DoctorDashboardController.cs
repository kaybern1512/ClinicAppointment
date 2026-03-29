using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Doctor;
using ClinicBookingMVC.ViewModels;
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
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdStr, out var userId))
                {
                    _logger.LogWarning("Could not parse user ID for user {User}", User.Identity.Name);
                    ViewData["ErrorMessage"] = "Không thể xác thực ID người dùng.";
                    return View("Error", new ClinicBookingMVC.ViewModels.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

                }

                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);

                if (doctor == null)
                {
                    _logger.LogWarning("User with ID {UserId} and role Doctor does not have an associated Doctor entity.", userId);
                    ViewData["ErrorMessage"] = "Không tìm thấy thông tin bác sĩ tương ứng với tài khoản của bạn.";
                    return View("Error", new ClinicBookingMVC.ViewModels.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

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
                return View("Error", new ClinicBookingMVC.ViewModels.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
