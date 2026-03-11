using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class Specialty
{
    [Key]
    public int SpecialtyId { get; set; }

    [StringLength(150)]
    public string SpecialtyName { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public bool IsFeatured { get; set; }

    [InverseProperty("Specialty")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Specialty")]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
