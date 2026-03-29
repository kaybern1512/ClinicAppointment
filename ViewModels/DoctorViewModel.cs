namespace ClinicBookingMVC.ViewModels
{
    public class DoctorViewModel
    {
        public int DoctorId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;

        public int ExperienceYears { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? WorkingTime { get; set; }

        public bool IsFeatured { get; set; }

        // 🔥 nên thêm
        public bool IsActive { get; set; }

        // 🔥 optional (nếu muốn hiển thị giá khám)
        public decimal? ConsultationFee { get; set; }
    }
}