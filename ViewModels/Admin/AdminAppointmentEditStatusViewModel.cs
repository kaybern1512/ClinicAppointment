using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicBookingMVC.ViewModels.Admin
{
    public class AdminAppointmentEditStatusViewModel
    {
        public int AppointmentId { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int StatusId { get; set; }

        public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();

        [Display(Name = "Ghi chú")]
        [StringLength(500)]
        public string? Note { get; set; }
    }
}