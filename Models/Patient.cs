using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("UserId", Name = "UQ__Patients__1788CC4DE5D24A9E", IsUnique = true)]
public partial class Patient
{
    [Key]
    public int PatientId { get; set; }

    public int UserId { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(150)]
    public string? EmergencyContactName { get; set; }

    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Patient")]
    public virtual User User { get; set; } = null!;
}
