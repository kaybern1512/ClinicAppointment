using System.ComponentModel.DataAnnotations;

namespace ClinicBookingMVC.Models
{
    public class DoctorRecommendation
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string SpecialtyName { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
        public decimal ConsultationFee { get; set; }
        public string? ImageUrl { get; set; }
    }
}

