using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ClinicBookingMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminDashboardController(ClinicBookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalDoctors = await _context.Doctors
                    .AsNoTracking()
                    .CountAsync(d => d.IsActive),

                TotalSpecialties = await _context.Specialties
                    .AsNoTracking()
                    .CountAsync(s => s.IsActive),

                TotalAppointments = await _context.Appointments
                    .AsNoTracking()
                    .CountAsync(),

                TotalUsers = await _context.Users
                    .AsNoTracking()
                    .CountAsync(u => u.IsActive),

                PendingAppointments = await _context.Appointments
                    .AsNoTracking()
                    .CountAsync(a => a.Status.StatusName == "Pending"),

                ConfirmedAppointments = await _context.Appointments
                    .AsNoTracking()
                    .CountAsync(a => a.Status.StatusName == "Confirmed"),

                CompletedAppointments = await _context.Appointments
                    .AsNoTracking()
                    .CountAsync(a => a.Status.StatusName == "Completed"),

                CancelledAppointments = await _context.Appointments
                    .AsNoTracking()
                    .CountAsync(a => a.Status.StatusName == "Cancelled"),

                RecentAppointments = await _context.Appointments
                    .AsNoTracking()
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
                    .AsNoTracking()
                    .Include(d => d.Specialty)
                    .Where(d => d.IsFeatured && d.IsActive)
                    .OrderBy(d => d.FullName)
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