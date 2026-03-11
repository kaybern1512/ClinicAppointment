using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminDoctorCreateViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Chuyên khoa")]
        public int SpecialtyId { get; set; }

        [Range(0, 60)]
        [Display(Name = "Số năm kinh nghiệm")]
        public int ExperienceYears { get; set; }

        [StringLength(2000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(255)]
        [Display(Name = "Ảnh URL")]
        public string? ImageUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Lịch làm việc")]
        public string? WorkingTime { get; set; }

        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;

        public IEnumerable<SelectListItem> Specialties { get; set; } = new List<SelectListItem>();
    }
}