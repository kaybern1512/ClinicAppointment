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

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    public virtual DbSet<DoctorReview> DoctorReviews { get; set; }
    public virtual DbSet<FamilyMember> FamilyMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC243077697");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Doctors");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments).HasConstraintName("FK_Appointments_Users");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_Specialties");

            entity.HasOne(d => d.Status).WithMany(p => p.Appointments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointments_AppointmentStatuses");
        });

        modelBuilder.Entity<AppointmentStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK__Appointm__C8EE206331249261");
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.ContactMessageId).HasName("PK__ContactM__2B0D4DFC3ADFF039");

            entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EBF097690A9");

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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A59CDC53F");
        });



        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F6A86F916402");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId);

            // Explicitly define the one-to-many relationship with Doctor
            entity.HasOne(d => d.Doctor)
                  .WithMany(p => p.TimeSlots)
                  .HasForeignKey(d => d.DoctorId)
                  .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C427F2F31");

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
