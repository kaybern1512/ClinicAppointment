using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminScheduleEditViewModel
    {
        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }

        public string DoctorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày làm việc.")]
        [Display(Name = "Ngày làm việc")]
        public DateOnly WorkDate { get; set; }

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "Có sẵn")]
        public bool IsAvailable { get; set; }
    }
}
