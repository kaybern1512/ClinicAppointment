namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminSpecialtyListItemViewModel
    {
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
    }
}