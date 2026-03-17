using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ClinicBookingMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ClinicBookingContext _context;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(ClinicBookingContext context, ILogger<AdminDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Admin Dashboard Index page accessed.");
            var model = new AdminDashboardViewModel();
            try
            {
                model.TotalDoctors = await _context.Doctors.CountAsync();
                model.TotalSpecialties = await _context.Specialties.CountAsync();
                model.TotalAppointments = await _context.Appointments.CountAsync();
                model.TotalUsers = await _context.Users.CountAsync();
                model.PendingAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Pending");
                model.ConfirmedAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Confirmed");
                model.CompletedAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Completed");
                model.CancelledAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Cancelled");
                model.RecentAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Specialty)
                    .Include(a => a.Status)
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .Select(a => new AdminAppointmentListItemViewModel
                    {
                        AppointmentId = a.AppointmentId,
                        PatientName = a.PatientName,
                        DoctorName = a.Doctor.FullName,
                        SpecialtyName = a.Specialty.SpecialtyName,
                        AppointmentDate = a.AppointmentDate,
                        AppointmentTime = a.AppointmentTime,
                        StatusName = a.Status.StatusName,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();
                model.FeaturedDoctors = await _context.Doctors
                    .Include(d => d.Specialty)
                    .Where(d => d.IsFeatured)
                    .Take(4)
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

                // Prepare data for the chart (Appointments in the last 7 days)
                var today = DateOnly.FromDateTime(DateTime.Today);
                var last7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).Reverse().ToList();
                var appointmentCounts = await _context.Appointments
                    .Where(a => a.AppointmentDate >= last7Days.First() && a.AppointmentDate <= last7Days.Last())
                    .GroupBy(a => a.AppointmentDate)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Date, x => x.Count);

                var chartLabels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
                var chartData = last7Days.Select(d => appointmentCounts.ContainsKey(d) ? appointmentCounts[d] : 0).ToList();

                ViewBag.ChartLabels = JsonSerializer.Serialize(chartLabels);
                ViewBag.ChartData = JsonSerializer.Serialize(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching data for the Admin Dashboard.");
                // Optionally, you can set an error message in ViewBag or return a different view
                ViewBag.ErrorMessage = "Đã có lỗi xảy ra khi tải dữ liệu cho dashboard.";
            }


            return View(model);
        }
    }
}