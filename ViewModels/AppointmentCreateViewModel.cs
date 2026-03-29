using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicBookingMVC.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(150)]
        [Display(Name = "Họ và tên")]
        public string PatientName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateOnly? DateOfBirth { get; set; }

        [StringLength(20)]
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
        [Display(Name = "Chuyên khoa")]
        public int SpecialtyId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn bác sĩ.")]
        [Display(Name = "Bác sĩ")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khám.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khám")]
        public DateOnly AppointmentDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khung giờ khám.")]
        [Display(Name = "Khung giờ khám")]
        public int DoctorScheduleSlotId { get; set; }

        [StringLength(1000)]
        [Display(Name = "Triệu chứng / Ghi chú")]
        public string? Symptoms { get; set; }

        public IEnumerable<SelectListItem> Specialties { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Doctors { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> ScheduleSlots { get; set; } = new List<SelectListItem>();
    }
}