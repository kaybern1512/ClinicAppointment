using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("StatusName", Name = "UQ__Appointm__05E7698A98D7986F", IsUnique = true)]
public partial class AppointmentStatus
{
    [Key]
    public int StatusId { get; set; }

    [StringLength(50)]
    public string StatusName { get; set; } = null!;

    [InverseProperty("NewStatus")]
    public virtual ICollection<AppointmentStatusHistory> AppointmentStatusHistoryNewStatuses { get; set; } = new List<AppointmentStatusHistory>();

    [InverseProperty("OldStatus")]
    public virtual ICollection<AppointmentStatusHistory> AppointmentStatusHistoryOldStatuses { get; set; } = new List<AppointmentStatusHistory>();

    [InverseProperty("Status")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
