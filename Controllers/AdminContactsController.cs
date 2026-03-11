using ClinicBookingMVC.Models;
using ClinicBookingMVC.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Controllers
{
    public class AdminContactsController : Controller
    {
        private readonly ClinicBookingContext _context;

        public AdminContactsController(ClinicBookingContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.ContactMessages
                .OrderByDescending(c => c.SentAt)
                .Select(c => new AdminContactListItemViewModel
                {
                    ContactMessageId = c.ContactMessageId,
                    FullName = c.FullName,
                    Email = c.Email,
                    MessagePreview = c.Message.Length > 60 ? c.Message.Substring(0, 60) + "..." : c.Message,
                    SentAt = c.SentAt
                })
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contact = await _context.ContactMessages.FirstOrDefaultAsync(c => c.ContactMessageId == id);
            if (contact == null) return NotFound();

            var model = new AdminContactDetailsViewModel
            {
                ContactMessageId = contact.ContactMessageId,
                FullName = contact.FullName,
                Email = contact.Email,
                Message = contact.Message,
                SentAt = contact.SentAt
            };

            return View(model);
        }
    }
}