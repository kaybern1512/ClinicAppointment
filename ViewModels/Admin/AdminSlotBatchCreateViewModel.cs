using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminSlotBatchCreateViewModel
    {
        public int ScheduleId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateOnly WorkDate { get; set; }

        [Display(Name = "Số lượng tối đa trên mỗi slot")]
        [Range(1, 20, ErrorMessage = "Số lượng phải từ 1 đến 20.")]
        public int MaxAppointments { get; set; } = 1;

        [Display(Name = "Chọn các khung giờ")]
        [Required(ErrorMessage = "Vui lòng chọn ít nhất một khung giờ.")]
        public List<int> SelectedTimeSlotIds { get; set; } = new();

        public IEnumerable<SelectListItem> AvailableTimeSlots { get; set; } = new List<SelectListItem>();
    }
}
