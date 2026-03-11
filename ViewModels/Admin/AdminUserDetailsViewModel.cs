namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminUserDetailsViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<AdminAppointmentListItemViewModel> Appointments { get; set; } = new();
    }
}