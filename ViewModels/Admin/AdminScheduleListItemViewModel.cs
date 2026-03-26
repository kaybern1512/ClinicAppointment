namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminScheduleListItemViewModel
    {
        public int ScheduleId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateOnly WorkDate { get; set; }
        public bool IsAvailable { get; set; }
        public int SlotCount { get; set; }
        public string? Notes { get; set; }
    }
}
