using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public int UserId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
