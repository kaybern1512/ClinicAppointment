using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class ClinicBookingContext : DbContext
{
    public ClinicBookingContext()
    {
    }

    public ClinicBookingContext(DbContextOptions<ClinicBookingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentStatus> AppointmentStatuses { get; set; }

    public virtual DbSet<AppointmentStatusHistory> AppointmentStatusHistories { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSchedule> DoctorSchedules { get; set; }

    public virtual DbSet<DoctorScheduleSlot> DoctorScheduleSlots { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    public virtual DbSet<DoctorReview> DoctorReviews { get; set; }
    public virtual DbSet<FamilyMember> FamilyMembers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC2616B6ACE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctors");

            entity.HasOne(d => d.DoctorScheduleSlot).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_DoctorScheduleSlots");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_DoctorSchedules");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Specialties");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_AppointmentStatuses");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_TimeSlots");

            entity.HasOne(d => d.UserPatient).WithMany(p => p.Appointments).HasConstraintName("FK_Appointments_Users");
        });

        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Appointm__C8EE2063FF069F87");
        });

        modelBuilder.Entity<AppointmentStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Appointm__4D7B4ABD4264157D");

            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentStatusHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusHistory_Appointments");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.AppointmentStatusHistories).HasConstraintName("FK_StatusHistory_Users");

            entity.HasOne(d => d.NewStatus).WithMany(p => p.AppointmentStatusHistoryNewStatuses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusHistory_NewStatus");

            entity.HasOne(d => d.OldStatus).WithMany(p => p.AppointmentStatusHistoryOldStatuses).HasConstraintName("FK_StatusHistory_OldStatus");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.ContactMessageId).HasName("PK__ContactM__2B0D4DFCC3A37FA2");

            entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EBF6CC17F2B");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Specialties");
            // The Doctors table does not actually have a UserId column in the database schema.
            // Explicitly defining this one-to-one relationship causes "Invalid column name 'UserId'" runtime errors.
            // entity.HasOne(d => d.User)
            //       .WithOne(p => p.Doctor)
            //       .HasForeignKey<Doctor>(d => d.UserId)
            //       .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__DoctorSc__9C8A5B49B944F90A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedules)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorSchedules_Doctors");
        });

        modelBuilder.Entity<DoctorScheduleSlot>(entity =>
        {
            entity.HasKey(e => e.DoctorScheduleSlotId).HasName("PK__DoctorSc__14C4F2DE15EC6A1D");

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.MaxAppointments).HasDefaultValue(1);

            entity.HasOne(d => d.Schedule).WithMany(p => p.DoctorScheduleSlots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorScheduleSlots_Schedules");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.DoctorScheduleSlots)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorScheduleSlots_TimeSlots");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC366E68E0310");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38A928FCC6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Appointments");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AD3B39B06");
        });



        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F6A8FD3FF84C");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId).HasName("PK__TimeSlot__41CC1F32F3A121BF");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });



        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDD68CD99");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Appointment)
                  .WithMany()
                  .HasForeignKey(d => d.AppointmentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DoctorReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Doctor)
                  .WithMany()
                  .HasForeignKey(d => d.DoctorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Patient)
                  .WithMany()
                  .HasForeignKey(d => d.PatientId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.Appointment)
                  .WithMany()
                  .HasForeignKey(d => d.AppointmentId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.HasOne(d => d.Patient)
                  .WithMany()
                  .HasForeignKey(d => d.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
