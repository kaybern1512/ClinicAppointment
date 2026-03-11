using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminDashboardController(ClinicBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalSpecialties = await _context.Specialties.CountAsync(),
                TotalAppointments = await _context.Appointments.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                PendingAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Pending"),
                ConfirmedAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Confirmed"),
                CompletedAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Completed"),
                CancelledAppointments = await _context.Appointments.CountAsync(a => a.Status.StatusName == "Cancelled"),
                RecentAppointments = await _context.Appointments
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
                    .ToListAsync(),
                FeaturedDoctors = await _context.Doctors
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
                    .ToListAsync()
            };

            return View(model);
        }
    }
}