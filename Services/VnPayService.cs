using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;

namespace ClinicBookingMVC.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(int appointmentId, decimal amount, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            
            var urlCallBack = _config["VnPay:ReturnUrl"];
            if (!string.IsNullOrEmpty(urlCallBack) && urlCallBack.StartsWith("/"))
            {
                urlCallBack = $"{context.Request.Scheme}://{context.Request.Host}{urlCallBack}";
            }

            pay.AddRequestData("vnp_Version", _config["VnPay:Version"] ?? "2.1.0");
            pay.AddRequestData("vnp_Command", _config["VnPay:Command"] ?? "pay");
            pay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)(amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"] ?? "VND");
            
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }
            pay.AddRequestData("vnp_IpAddr", ipAddress);
            
            pay.AddRequestData("vnp_Locale", _config["VnPay:Locale"] ?? "vn");
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toan coc cho lich kham {appointmentId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", $"{appointmentId}_{tick}");

            var paymentUrl = pay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public (bool IsSuccess, int AppointmentId, string TransactionCode, decimal Amount) ValidateReturn(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = pay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = pay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collections["vnp_SecureHash"];
            var vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode");
            var vnp_AmountRaw = pay.GetResponseData("vnp_Amount");

            bool checkSignature = pay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);

            if (!checkSignature)
            {
                return (false, 0, string.Empty, 0);
            }

            int appointmentId = 0;
            if (!string.IsNullOrEmpty(vnp_orderId))
            {
                var parts = vnp_orderId.Split('_');
                if (parts.Length > 0)
                {
                    int.TryParse(parts[0], out appointmentId);
                }
            }
            
            decimal amount = 0;
            if (!string.IsNullOrEmpty(vnp_AmountRaw) && decimal.TryParse(vnp_AmountRaw, out decimal rawAmount))
            {
                amount = rawAmount / 100;
            }

            if (vnp_ResponseCode == "00")
            {
                return (true, appointmentId, vnp_TransactionId, amount);
            }

            return (false, appointmentId, vnp_TransactionId, amount);
        }
    }
}
