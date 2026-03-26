namespace ClinicBookingMVC.ViewModels
{
    public class AppointmentListItemViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? PatientPhoneNumber { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string BookingCode { get; set; } = string.Empty;
    }
}