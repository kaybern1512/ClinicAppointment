using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class AppointmentStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<AppointmentStatusHistory> AppointmentStatusHistoryNewStatuses { get; set; } = new List<AppointmentStatusHistory>();

    public virtual ICollection<AppointmentStatusHistory> AppointmentStatusHistoryOldStatuses { get; set; } = new List<AppointmentStatusHistory>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
