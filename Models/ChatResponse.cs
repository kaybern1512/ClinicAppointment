namespace ClinicBookingMVC.Models
{
    public class ChatResponse
    {
        public string Response { get; set; } = string.Empty;
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
        
        // Structured recommendation fields for chatbox
        public int? RecommendedSpecialtyId { get; set; }
        public string RecommendedSpecialtyName { get; set; } = string.Empty;
        public int? RecommendedDoctorId { get; set; }
        public string RecommendedDoctorName { get; set; } = string.Empty;
        public string RecommendationReason { get; set; } = string.Empty;
        public List<DoctorRecommendation> AvailableDoctors { get; set; } = new();
    }
}

