using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Table("AppointmentStatusHistory")]
public partial class AppointmentStatusHistory
{
    [Key]
    public int HistoryId { get; set; }

    public int AppointmentId { get; set; }

    public int? OldStatusId { get; set; }

    public int NewStatusId { get; set; }

    public int? ChangedByUserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ChangedAt { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("AppointmentStatusHistories")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("ChangedByUserId")]
    [InverseProperty("AppointmentStatusHistories")]
    public virtual User? ChangedByUser { get; set; }

    [ForeignKey("NewStatusId")]
    [InverseProperty("AppointmentStatusHistoryNewStatuses")]
    public virtual AppointmentStatus NewStatus { get; set; } = null!;

    [ForeignKey("OldStatusId")]
    [InverseProperty("AppointmentStatusHistoryOldStatuses")]
    public virtual AppointmentStatus? OldStatus { get; set; }
}
