using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminSlotCreateViewModel
    {
        public int ScheduleId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khung giờ.")]
        [Display(Name = "Khung giờ")]
        public int TimeSlotId { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Số lượng tối đa phải từ 1 đến 20.")]
        [Display(Name = "Số lượng tối đa")]
        public int MaxAppointments { get; set; } = 1;

        public string? DoctorName { get; set; }
        public DateOnly WorkDate { get; set; }

        public IEnumerable<SelectListItem> TimeSlots { get; set; } = new List<SelectListItem>();
    }
}
