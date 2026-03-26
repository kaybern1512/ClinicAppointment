using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("DoctorId", "WorkDate", Name = "UQ_DoctorSchedules_Doctor_WorkDate", IsUnique = true)]
public partial class DoctorSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly WorkDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public bool IsAvailable { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Schedule")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("DoctorId")]
    [InverseProperty("DoctorSchedules")]
    public virtual Doctor Doctor { get; set; } = null!;

    [InverseProperty("Schedule")]
    public virtual ICollection<DoctorScheduleSlot> DoctorScheduleSlots { get; set; } = new List<DoctorScheduleSlot>();
}
