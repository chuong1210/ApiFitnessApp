using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Helpers;
namespace Infrastructure.Services
{

    public class VnpayService : IVnpayService
    {
        private readonly VnpaySettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor; // Lấy IP

        public VnpayService(IOptions<VnpaySettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GeneratePaymentUrl(decimal amount, string orderId, string orderInfo, HttpContext httpContext)
        {
            var pay = new VnpayLibrary();

            // Thêm các tham số cần thiết vào VnpayLibrary
            pay.AddRequestData("vnp_Version", _settings.Version);
            pay.AddRequestData("vnp_Command", _settings.Command);
            pay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            pay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString()); // VNPAY yêu cầu Amount * 100
            pay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _settings.CurrCode);
            pay.AddRequestData("vnp_IpAddr", GetIpAddress(httpContext) ?? "127.0.0.1"); // Lấy IP từ request
            pay.AddRequestData("vnp_Locale", _settings.Locale);
            pay.AddRequestData("vnp_OrderInfo", orderInfo);
            pay.AddRequestData("vnp_OrderType", "other"); // Hoặc loại phù hợp: billpayment, fashion, etc.
            pay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            pay.AddRequestData("vnp_TxnRef", orderId); // Mã tham chiếu của merchant

            // --- Tạo URL thanh toán ---
            string paymentUrl = pay.CreateRequestUrl(_settings.BaseUrl, _settings.HashSecret);
            return paymentUrl;
        }

        public bool ValidateSignature(IQueryCollection vnpayData, string inputHash)
        {
            var pay = new VnpayLibrary();
            foreach (var kvp in vnpayData)
            {
                if (!string.IsNullOrEmpty(kvp.Key) && kvp.Key.StartsWith("vnp_") && kvp.Key != "vnp_SecureHash")
                {
                    // Loại bỏ dấu '+' có thể được mã hóa thành khoảng trắng khi query string
                    var value = kvp.Value.ToString().Replace('+', ' ');
                    pay.AddResponseData(kvp.Key, value);
                }
            }

            // Sắp xếp và tạo chuỗi dữ liệu để kiểm tra hash
            string checkData = pay.GetResponseDataForChecksum(_settings.HashSecret);
            // Tạo hash từ dữ liệu nhận được
            string calculatedHash = VnpayLibrary.HmacSHA512(_settings.HashSecret, checkData);

            // So sánh hash nhận được với hash tính toán
            return calculatedHash.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        // Helper lấy địa chỉ IP
        private string? GetIpAddress(HttpContext context)
        {
            string? ipAddress = context.Connection.RemoteIpAddress?.ToString();
            if (ipAddress == "::1") // Localhost IPv6
            {
                ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                               .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
            }
            return ipAddress;
        }
    }
}
