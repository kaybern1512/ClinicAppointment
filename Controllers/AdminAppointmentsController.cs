using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminAppointmentsController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminAppointmentsController(ClinicBookingContext context)
        {
            _context = context;
        }

        private async Task LoadStatusesAsync(AdminAppointmentEditStatusViewModel model)
        {
            model.Statuses = await _context.AppointmentStatuses
                .Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.StatusName
                })
                .ToListAsync();
        }

        public async Task<IActionResult> Index(
            string? keyword,
            int? statusId,
            int? specialtyId,
            int? doctorId,
            DateOnly? appointmentDate)
        {
            var query = _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a =>
                    a.PatientName.Contains(keyword) ||
                    a.AppointmentId.ToString().Contains(keyword));
            }

            if (statusId.HasValue && statusId > 0)
            {
                query = query.Where(a => a.StatusId == statusId.Value);
            }

            if (specialtyId.HasValue && specialtyId > 0)
            {
                query = query.Where(a => a.SpecialtyId == specialtyId.Value);
            }

            if (doctorId.HasValue && doctorId > 0)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }

            if (appointmentDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate == appointmentDate.Value);
            }

            ViewBag.Keyword = keyword;
            ViewBag.StatusId = new SelectList(await _context.AppointmentStatuses.ToListAsync(), "StatusId", "StatusName", statusId);
            ViewBag.SpecialtyId = new SelectList(await _context.Specialties.ToListAsync(), "SpecialtyId", "SpecialtyName", specialtyId);
            ViewBag.DoctorId = new SelectList(await _context.Doctors.ToListAsync(), "DoctorId", "FullName", doctorId);
            ViewBag.AppointmentDate = appointmentDate?.ToString("yyyy-MM-dd");

            var model = await query
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

            if (appointment == null) return NotFound();

            var model = new AdminAppointmentDetailsViewModel
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
                CreatedAt = appointment.CreatedAt
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditStatus(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var model = new AdminAppointmentEditStatusViewModel
            {
                AppointmentId = appointment.AppointmentId,
                StatusId = appointment.StatusId
            };

            await LoadStatusesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(AdminAppointmentEditStatusViewModel model)
        {
            var appointment = await _context.Appointments.FindAsync(model.AppointmentId);
            if (appointment == null) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadStatusesAsync(model);
                return View(model);
            }

            appointment.StatusId = model.StatusId;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật trạng thái lịch hẹn thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}