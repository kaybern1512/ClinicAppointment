namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalDoctors { get; set; }
        public int TotalSpecialties { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalUsers { get; set; }

        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }

        public List<AdminAppointmentListItemViewModel> RecentAppointments { get; set; } = new();
        public List<AdminDoctorListItemViewModel> FeaturedDoctors { get; set; } = new();
    }
}