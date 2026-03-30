using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class TimeSlot
{
    [Key]
    public int TimeSlotId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [StringLength(50)]
    public string? SlotLabel { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("TimeSlot")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("TimeSlot")]
    public virtual ICollection<DoctorScheduleSlot> DoctorScheduleSlots { get; set; } = new List<DoctorScheduleSlot>();
}
