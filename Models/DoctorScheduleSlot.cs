using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("ScheduleId", "TimeSlotId", Name = "UQ_DoctorScheduleSlots", IsUnique = true)]
public partial class DoctorScheduleSlot
{
    [Key]
    public int DoctorScheduleSlotId { get; set; }

    public int ScheduleId { get; set; }

    public int TimeSlotId { get; set; }

    public int MaxAppointments { get; set; }

    public int CurrentAppointments { get; set; }

    public bool IsAvailable { get; set; }

    [InverseProperty("DoctorScheduleSlot")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("ScheduleId")]
    [InverseProperty("DoctorScheduleSlots")]
    public virtual DoctorSchedule Schedule { get; set; } = null!;

    [ForeignKey("TimeSlotId")]
    [InverseProperty("DoctorScheduleSlots")]
    public virtual TimeSlot TimeSlot { get; set; } = null!;
}
