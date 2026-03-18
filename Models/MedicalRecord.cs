using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBookingMVC.Models;

public partial class MedicalRecord
{
    [Key]
    public int RecordId { get; set; }

    public int AppointmentId { get; set; }

    [StringLength(2000)]
    public string? Diagnosis { get; set; }

    [StringLength(2000)]
    public string? Prescription { get; set; }

    [StringLength(2000)]
    public string? DoctorNotes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("AppointmentId")]
    public virtual Appointment Appointment { get; set; } = null!;
}
