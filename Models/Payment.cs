using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int AppointmentId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    [StringLength(50)]
    public string PaymentStatus { get; set; } = null!;

    [StringLength(100)]
    public string? TransactionCode { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? PaidAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Payments")]
    public virtual Appointment Appointment { get; set; } = null!;
}
