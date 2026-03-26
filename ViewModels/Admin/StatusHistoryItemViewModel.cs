namespace ClinicBookingMVC.ViewModels.Admin
{
    public class StatusHistoryItemViewModel
    {
        public string? OldStatusName { get; set; }
        public string NewStatusName { get; set; } = null!;
        public string? ChangedByName { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Note { get; set; }
    }
}
