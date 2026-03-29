using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    [Authorize(Roles = "Admin")]
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

        private async Task LoadRolesAsync(AdminUserEditViewModel model)
        {
            model.Roles = await _context.Roles
                .AsNoTracking()
                .Select(r => new SelectListItem
                {
                    Value = r.RoleId.ToString(),
                    Text = r.RoleName
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            var model = new AdminUserEditViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            };

            await LoadRolesAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel model)
        {
            model.FullName = model.FullName?.Trim();
            model.Email = model.Email?.Trim();
            model.PhoneNumber = model.PhoneNumber?.Trim();

            if (!ModelState.IsValid)
            {
                await LoadRolesAsync(model);
                return View(model);
            }

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null) return NotFound();

            user.FullName = model.FullName!;
            user.Email = model.Email!;
            user.PhoneNumber = model.PhoneNumber!;
            user.RoleId = model.RoleId;
            user.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vô hiệu hóa người dùng thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
