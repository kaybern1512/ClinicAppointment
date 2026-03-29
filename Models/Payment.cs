using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int AppointmentId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
