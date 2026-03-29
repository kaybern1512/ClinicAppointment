using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class TimeSlot
{
    public int TimeSlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? SlotLabel { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DoctorScheduleSlot> DoctorScheduleSlots { get; set; } = new List<DoctorScheduleSlot>();
}
