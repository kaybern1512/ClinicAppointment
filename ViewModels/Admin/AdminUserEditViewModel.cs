using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminUserEditViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListItem> Roles { get; set; } = new();
    }
}

