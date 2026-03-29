using System;
using System.Collections.Generic;

namespace ClinicBookingMVC.Models;

public partial class ContactMessage
{
    public int ContactMessageId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsReplied { get; set; }
}
