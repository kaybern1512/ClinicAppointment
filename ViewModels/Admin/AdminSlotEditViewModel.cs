using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminSlotEditViewModel
    {
        public int DoctorScheduleSlotId { get; set; }
        public int ScheduleId { get; set; }

        public string TimeSlotLabel { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Số lượng tối đa phải từ 1 đến 20.")]
        [Display(Name = "Số lượng tối đa")]
        public int MaxAppointments { get; set; }

        public int CurrentAppointments { get; set; }

        [Display(Name = "Có sẵn")]
        public bool IsAvailable { get; set; }
    }
}
