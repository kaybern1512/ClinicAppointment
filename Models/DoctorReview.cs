using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBookingMVC.Models;

public partial class DoctorReview
{
    [Key]
    public int ReviewId { get; set; }

    public int DoctorId { get; set; }

    public int PatientId { get; set; }

    public int AppointmentId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("DoctorId")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    public virtual User Patient { get; set; } = null!;

    [ForeignKey("AppointmentId")]
    public virtual Appointment Appointment { get; set; } = null!;
}
