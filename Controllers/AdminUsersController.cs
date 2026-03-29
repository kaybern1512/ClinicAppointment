using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminUsersController(ClinicBookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminUserListItemViewModel
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    RoleName = u.Role.RoleName,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(u => u.Appointments)
                    .ThenInclude(a => a.Specialty)
                .Include(u => u.Appointments)
                    .ThenInclude(a => a.Status)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminUserDetailsViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role.RoleName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Appointments = user.Appointments
                    .OrderByDescending(a => a.CreatedAt)
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
                    .ToList()
            };

            return View(model);
        }
    }
}