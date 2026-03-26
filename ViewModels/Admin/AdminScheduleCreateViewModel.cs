using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminScheduleCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn bác sĩ.")]
        [Display(Name = "Bác sĩ")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày làm việc.")]
        [Display(Name = "Ngày làm việc")]
        public DateOnly WorkDate { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Có sẵn")]
        public bool IsAvailable { get; set; } = true;

        public IEnumerable<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
    }
}
