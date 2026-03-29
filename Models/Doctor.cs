using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("UserId", Name = "UQ__Doctors__1788CC4D28940976", IsUnique = true)]
public partial class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    public int? UserId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    public int SpecialtyId { get; set; }

    public int ExperienceYears { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? Qualification { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [StringLength(255)]
    public string? WorkingTime { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ConsultationFee { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Doctor")]
    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    [ForeignKey("SpecialtyId")]
    [InverseProperty("Doctors")]
    public virtual Specialty Specialty { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Doctor")]
    public virtual User? User { get; set; }
}

