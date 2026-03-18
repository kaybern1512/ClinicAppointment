using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBookingMVC.Models;

public partial class FamilyMember
{
    [Key]
    public int MemberId { get; set; }

    public int PatientId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Gender { get; set; }

    [StringLength(50)]
    public string Relationship { get; set; } = null!; // Ví dụ: Bố, Mẹ, Con trai, Con gái, Vợ, Chồng...

    [ForeignKey("PatientId")]
    public virtual User Patient { get; set; } = null!;
}
