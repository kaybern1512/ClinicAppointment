using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

[Index("Email", Name = "UQ__Users__A9D10534200FD58A", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
