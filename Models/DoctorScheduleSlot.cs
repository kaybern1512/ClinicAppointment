using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class DoctorScheduleSlot
{
    public int DoctorScheduleSlotId { get; set; }

    public int ScheduleId { get; set; }

    public int TimeSlotId { get; set; }

    public int MaxAppointments { get; set; }

    public int CurrentAppointments { get; set; }

    public bool IsAvailable { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual DoctorSchedule Schedule { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
