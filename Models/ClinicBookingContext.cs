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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-QK1O8VD5;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC287DD3A5B");

            entity.HasIndex(e => new { e.DoctorId, e.AppointmentDate, e.AppointmentTime }, "UQ_Appointments_Doctor_Date_Time").IsUnique();

            entity.HasIndex(e => e.BookingCode, "UQ__Appointm__C6E56BD50D2F27A5").IsUnique();

            entity.Property(e => e.BookingCode).HasMaxLength(30);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.Note).HasMaxLength(1000);
            entity.Property(e => e.PatientName).HasMaxLength(150);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Symptoms).HasMaxLength(1000);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctors");

            entity.HasOne(d => d.DoctorScheduleSlot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorScheduleSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_DoctorScheduleSlots");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_DoctorSchedules");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Specialties");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_AppointmentStatuses");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_TimeSlots");

            entity.HasOne(d => d.UserPatient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.UserPatientId)
                .HasConstraintName("FK_Appointments_Users");
        });

        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Appointm__C8EE2063E225C337");

            entity.HasIndex(e => e.StatusName, "UQ__Appointm__05E7698A802A6B49").IsUnique();

            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<AppointmentStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Appointm__4D7B4ABD123FD13C");

            entity.ToTable("AppointmentStatusHistory");

            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(500);

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentStatusHistories)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusHistory_Appointments");

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.AppointmentStatusHistories)
                .HasForeignKey(d => d.ChangedByUserId)
                .HasConstraintName("FK_StatusHistory_Users");

            entity.HasOne(d => d.NewStatus).WithMany(p => p.AppointmentStatusHistoryNewStatuses)
                .HasForeignKey(d => d.NewStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StatusHistory_NewStatus");

            entity.HasOne(d => d.OldStatus).WithMany(p => p.AppointmentStatusHistoryOldStatuses)
                .HasForeignKey(d => d.OldStatusId)
                .HasConstraintName("FK_StatusHistory_OldStatus");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.ContactMessageId).HasName("PK__ContactM__2B0D4DFC60F62CC8");

            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.Message).HasMaxLength(2000);
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EBF81C561BF");

            entity.HasIndex(e => e.UserId, "UQ__Doctors__1788CC4DA25C51A1").IsUnique();

            entity.Property(e => e.ConsultationFee).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Qualification).HasMaxLength(255);
            entity.Property(e => e.WorkingTime).HasMaxLength(255);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctors_Specialties");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .HasConstraintName("FK_Doctors_Users");
        });

        modelBuilder.Entity<DoctorSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__DoctorSc__9C8A5B49C449389B");

            entity.HasIndex(e => new { e.DoctorId, e.WorkDate }, "UQ_DoctorSchedules_Doctor_WorkDate").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSchedules)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorSchedules_Doctors");
        });

        modelBuilder.Entity<DoctorScheduleSlot>(entity =>
        {
            entity.HasKey(e => e.DoctorScheduleSlotId).HasName("PK__DoctorSc__14C4F2DEACABF006");

            entity.HasIndex(e => new { e.ScheduleId, e.TimeSlotId }, "UQ_DoctorScheduleSlots").IsUnique();

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.MaxAppointments).HasDefaultValue(1);

            entity.HasOne(d => d.Schedule).WithMany(p => p.DoctorScheduleSlots)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorScheduleSlots_Schedules");

            entity.HasOne(d => d.TimeSlot).WithMany(p => p.DoctorScheduleSlots)
                .HasForeignKey(d => d.TimeSlotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorScheduleSlots_TimeSlots");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC366192BD711");

            entity.HasIndex(e => e.UserId, "UQ__Patients__1788CC4D8670EE54").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmergencyContactName).HasMaxLength(150);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20);
            entity.Property(e => e.Gender).HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Patient)
                .HasForeignKey<Patient>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patients_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A38F41657D4");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidAt).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PaymentStatus).HasMaxLength(50);
            entity.Property(e => e.TransactionCode).HasMaxLength(100);

            entity.HasOne(d => d.Appointment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Appointments");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A01E99A52");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61602168551B").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F6A83C071135");

            entity.HasIndex(e => e.SpecialtyName, "UQ__Specialt__7DCA574899C0575E").IsUnique();

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(255);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SpecialtyName).HasMaxLength(150);
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId).HasName("PK__TimeSlot__41CC1F3228E88C35");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.SlotLabel).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C0D5152CC");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534189016C5").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
