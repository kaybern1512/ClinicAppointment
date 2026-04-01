namespace ClinicBookingMVC.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly AppointmentTime { get; set; }
        public string? Symptoms { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string BookingCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsDepositPaid { get; set; }
    }
}