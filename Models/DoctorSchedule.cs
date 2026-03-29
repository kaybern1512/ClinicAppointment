using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class DoctorSchedule
{
    public int ScheduleId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly WorkDate { get; set; }

    public string? Notes { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<DoctorScheduleSlot> DoctorScheduleSlots { get; set; } = new List<DoctorScheduleSlot>();
}
