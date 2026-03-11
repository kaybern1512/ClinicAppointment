using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminSpecialtyEditViewModel
    {
        public int SpecialtyId { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Tên chuyên khoa")]
        public string SpecialtyName { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Icon")]
        public string? Icon { get; set; }

        [StringLength(255)]
        [Display(Name = "Ảnh URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; }
    }
}