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

        public AppointmentsController(ClinicBookingContext context)
        {
            _context = context;
        }

        private async Task LoadDropdowns(AppointmentCreateViewModel model)
        {
            model.Specialties = await _context.Specialties
                .Select(s => new SelectListItem
                {
                    Value = s.SpecialtyId.ToString(),
                    Text = s.SpecialtyName
                })
                .ToListAsync();

            model.Doctors = await _context.Doctors
                .Where(d => d.IsActive)
                .Select(d => new SelectListItem
                {
                    Value = d.DoctorId.ToString(),
                    Text = d.FullName
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AppointmentCreateViewModel
            {
                AppointmentDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                AppointmentTime = new TimeOnly(8, 0)
            };

            await LoadDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (model.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("AppointmentDate", "Ngày khám phải từ hôm nay trở đi.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(model);
                return View(model);
            }

            int? patientId = HttpContext.Session.GetInt32("UserId");

            var appointment = new Appointment
            {
                PatientId = patientId,
                PatientName = model.PatientName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                SpecialtyId = model.SpecialtyId,
                DoctorId = model.DoctorId,
                AppointmentDate = model.AppointmentDate,
                AppointmentTime = model.AppointmentTime,
                Symptoms = model.Symptoms,
                StatusId = 1,
                CreatedAt = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt lịch thành công.";
            return RedirectToAction(nameof(MyAppointments));
        }

        public async Task<IActionResult> MyAppointments()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(email))
            {
                query = query.Where(a => a.Email == email);
            }

            var model = await query
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
                    StatusName = a.Status.StatusName
                })
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

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
                StatusName = appointment.Status.StatusName
            };

            return View(model);
        }
    }
}