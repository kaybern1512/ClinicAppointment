using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Nội dung")]
        public string Message { get; set; } = string.Empty;
    }
}