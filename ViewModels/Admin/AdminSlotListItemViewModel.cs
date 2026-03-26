namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminSlotListItemViewModel
    {
        public int DoctorScheduleSlotId { get; set; }
        public string TimeSlotLabel { get; set; } = string.Empty;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxAppointments { get; set; }
        public int CurrentAppointments { get; set; }
        public bool IsAvailable { get; set; }
    }
}
