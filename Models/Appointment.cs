using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    public int? PatientId { get; set; }

    [StringLength(150)]
    public string PatientName { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    [StringLength(1000)]
    public string? Symptoms { get; set; }

    public int StatusId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual User? Patient { get; set; }

    [ForeignKey("SpecialtyId")]
    [InverseProperty("Appointments")]
    public virtual Specialty Specialty { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("Appointments")]
    public virtual AppointmentStatus Status { get; set; } = null!;
}
