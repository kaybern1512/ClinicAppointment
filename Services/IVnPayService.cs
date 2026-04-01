using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ClinicBookingMVC.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(int appointmentId, decimal amount, HttpContext context);
        (bool IsSuccess, int AppointmentId, string TransactionCode, decimal Amount) ValidateReturn(IQueryCollection collections);
    }
}
