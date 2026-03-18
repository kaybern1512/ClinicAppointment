using ClinicBookingMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class PatientProfileController : Controller
    {
        private readonly ClinicBookingContext _context;

        public PatientProfileController(ClinicBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MyRecords()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách cuộc hẹn đã hoàn thành và có hồ sơ bệnh án
            var records = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .Where(a => a.Email == email && a.Status.StatusName == "Completed")
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();

            return View(records);
        }

        public async Task<IActionResult> RecordDetails(int id)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.Email == email);

            if (appointment == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            ViewBag.MedicalRecord = medicalRecord;

            return View(appointment);
        }
    }
}
