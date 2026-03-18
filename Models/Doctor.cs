using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    public int SpecialtyId { get; set; }

    public int ExperienceYears { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [StringLength(255)]
    public string? WorkingTime { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    
    [InverseProperty("Doctor")]
    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    [ForeignKey("SpecialtyId")]
    [InverseProperty("Doctors")]
    public virtual Specialty Specialty { get; set; } = null!;
}

