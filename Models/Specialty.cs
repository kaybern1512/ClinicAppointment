using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class Specialty
{
    public int SpecialtyId { get; set; }

    public string SpecialtyName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
