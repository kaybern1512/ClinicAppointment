using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? UserPatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public int DoctorId { get; set; }

    public int SpecialtyId { get; set; }

    public int ScheduleId { get; set; }

    public int TimeSlotId { get; set; }

    public int DoctorScheduleSlotId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string? Symptoms { get; set; }

    public string? Note { get; set; }

    public int StatusId { get; set; }

    public string BookingCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AppointmentStatusHistory> AppointmentStatusHistories { get; set; } = new List<AppointmentStatusHistory>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual DoctorScheduleSlot DoctorScheduleSlot { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual DoctorSchedule Schedule { get; set; } = null!;

    public virtual Specialty Specialty { get; set; } = null!;

    public virtual AppointmentStatus Status { get; set; } = null!;

    public virtual TimeSlot TimeSlot { get; set; } = null!;

    public virtual User? UserPatient { get; set; }
}
