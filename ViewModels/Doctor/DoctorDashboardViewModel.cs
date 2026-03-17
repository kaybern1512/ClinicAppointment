namespace ClinicBookingMVC.ViewModels.Doctor
{
    public class DoctorDashboardViewModel
    {
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int TotalAppointments { get; set; }

        public List<AppointmentListItemViewModel> UpcomingAppointments { get; set; } = new();
    }
}
