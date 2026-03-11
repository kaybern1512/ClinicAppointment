namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminContactDetailsViewModel
    {
        public int ContactMessageId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}