using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("DoctorId", "AppointmentDate", "AppointmentTime", Name = "UQ_Appointments_Doctor_Date_Time", IsUnique = true)]
[Index("BookingCode", Name = "UQ__Appointm__C6E56BD5E379392F", IsUnique = true)]
public partial class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    public int? UserPatientId { get; set; }

    [StringLength(150)]
    public string PatientName { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public int ScheduleId { get; set; }

    public int TimeSlotId { get; set; }

    public int DoctorScheduleSlotId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    [StringLength(1000)]
    public string? Symptoms { get; set; }

    [StringLength(1000)]
    public string? Note { get; set; }

    public int StatusId { get; set; }

    [StringLength(30)]
    public string BookingCode { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Appointment")]
    public virtual ICollection<AppointmentStatusHistory> AppointmentStatusHistories { get; set; } = new List<AppointmentStatusHistory>();

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("DoctorScheduleSlotId")]
    [InverseProperty("Appointments")]
    public virtual DoctorScheduleSlot DoctorScheduleSlot { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("ScheduleId")]
    [InverseProperty("Appointments")]
    public virtual DoctorSchedule Schedule { get; set; } = null!;

    [ForeignKey("SpecialtyId")]
    [InverseProperty("Appointments")]
    public virtual Specialty Specialty { get; set; } = null!;

    [ForeignKey("StatusId")]
    [InverseProperty("Appointments")]
    public virtual AppointmentStatus Status { get; set; } = null!;

    [ForeignKey("TimeSlotId")]
    [InverseProperty("Appointments")]
    public virtual TimeSlot TimeSlot { get; set; } = null!;

    [ForeignKey("UserPatientId")]
    [InverseProperty("Appointments")]
    public virtual User? UserPatient { get; set; }
}
