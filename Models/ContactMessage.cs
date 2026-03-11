using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinicBookingMVC.Models;

public partial class ContactMessage
{
    [Key]
    public int ContactMessageId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(2000)]
    public string Message { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime SentAt { get; set; }
}
