using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class AppointmentStatusHistory
{
    public int HistoryId { get; set; }

    public int AppointmentId { get; set; }

    public int? OldStatusId { get; set; }

    public int NewStatusId { get; set; }

    public int? ChangedByUserId { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? Note { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual User? ChangedByUser { get; set; }

    public virtual AppointmentStatus NewStatus { get; set; } = null!;

    public virtual AppointmentStatus? OldStatus { get; set; }
}
