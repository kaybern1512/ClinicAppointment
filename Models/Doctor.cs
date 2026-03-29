using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int? UserId { get; set; }

    public string FullName { get; set; } = null!;

    public int SpecialtyId { get; set; }

    public int ExperienceYears { get; set; }

    public string? Description { get; set; }

    public string? Qualification { get; set; }

    public string? ImageUrl { get; set; }

    public string? WorkingTime { get; set; }

    public decimal ConsultationFee { get; set; }

    public bool IsFeatured { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    public virtual Specialty Specialty { get; set; } = null!;

    public virtual User? User { get; set; }
}
