namespace ClinicBookingMVC.ViewModels
{
    public class HomeViewModel
    {
        public List<SpecialtyViewModel> FeaturedSpecialties { get; set; } = new();
        public List<DoctorViewModel> FeaturedDoctors { get; set; } = new();
    }
}