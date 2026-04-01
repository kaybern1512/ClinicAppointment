using ClinicBookingMVC.Models;
using ClinicBookingMVC.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicBookingMVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly ClinicBookingContext _context;

        public PaymentController(IVnPayService vnPayService, ClinicBookingContext context)
        {
            _vnPayService = vnPayService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePaymentUrl(int appointmentId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thực hiện thanh toán.";
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.UserPatientId == userId.Value);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lịch hẹn hoặc không có quyền truy cập.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            // Fixed deposit amount
            decimal depositAmount = 100000;

            var url = _vnPayService.CreatePaymentUrl(appointmentId, depositAmount, HttpContext);

            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> VnPayReturn()
        {
            var response = _vnPayService.ValidateReturn(Request.Query);

            if (!response.IsSuccess)
            {
                TempData["ErrorMessage"] = "Giao dịch thanh toán đã bị hủy hoặc không thành công.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == response.AppointmentId);

            if (appointment == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin lịch hẹn liên kết với thanh toán này.";
                return RedirectToAction("MyAppointments", "Appointments");
            }

            // Check if already paid
            bool isAlreadyPaid = await _context.Payments
                .AnyAsync(p => p.AppointmentId == response.AppointmentId && p.PaymentStatus == "Success");

            if (!isAlreadyPaid)
            {
                var payment = new Payment
                {
                    AppointmentId = response.AppointmentId,
                    Amount = response.Amount,
                    PaymentMethod = "VNPAY",
                    PaymentStatus = "Success",
                    TransactionCode = response.TransactionCode,
                    PaidAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thanh toán cọc thành công số tiền {response.Amount:N0} VNĐ.";
            }
            else
            {
                TempData["SuccessMessage"] = "Lịch hẹn này đã được thanh toán từ trước.";
            }

            return RedirectToAction("MyAppointments", "Appointments");
        }
    }
}
