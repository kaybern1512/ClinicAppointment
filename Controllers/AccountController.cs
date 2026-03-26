using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AccountController(ClinicBookingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email?.Trim() ?? string.Empty;
            string password = model.Password?.Trim() ?? string.Empty;

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.Password == password &&
                    u.IsActive);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserFullName", user.FullName);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("RoleName", user.Role.RoleName);

            TempData["SuccessMessage"] = "Đăng nhập thành công.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            model.Email = model.Email?.Trim()!;
            model.FullName = model.FullName?.Trim()!;
            model.PhoneNumber = model.PhoneNumber?.Trim()!;

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }
            if (await _context.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "PhoneNumber đã tồn tại.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var patientRoleId = await _context.Roles
                .Where(r => r.RoleName == "Patient")
                .Select(r => r.RoleId)
                .FirstOrDefaultAsync();

            if (patientRoleId == 0)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy role Patient trong hệ thống.");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    FullName = model.FullName!,
                    Email = model.Email!,
                    PhoneNumber = model.PhoneNumber!,
                    Password = model.Password!,
                    RoleId = patientRoleId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var patient = new Patient
                {
                    UserId = user.UserId,
                    CreatedAt = DateTime.Now
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                return RedirectToAction(nameof(Login));
            }
            catch
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Đăng ký thất bại. Vui lòng thử lại.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Index", "Home");
        }
    }
}