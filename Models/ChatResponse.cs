namespace ClinicBookingMVC.Models
{
    public class ChatResponse
    {
        public string Response { get; set; } = string.Empty;
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
