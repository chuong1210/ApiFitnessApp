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
using Microsoft.Extensions.Logging;
using System.Globalization;
using VNPay.NetCore;
namespace Infrastructure.Services
{

    public class VnpayService : IVnpayService
    {
        private readonly VnpaySettings _settings;
        private readonly IHttpContextAccessor _httpContextAccessor; // Lấy IP
        private readonly ILogger<VnpayService> _logger; // <<--- KHAI BÁO LOGGER

        public VnpayService(IOptions<VnpaySettings> settings, IHttpContextAccessor httpContextAccessor,ILogger<VnpayService> logger)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger;
        }

       // CreatePaymentUrlRequest
        public string GeneratePaymentUrl(decimal amount, string orderId, string orderInfo, HttpContext httpContext)
        {
            var pay = new VnpayLibrary();

            var cleanOrderInfo = RemoveDiacritics(orderInfo); // Dùng hàm helper để bỏ dấu tiếng Việt

            pay.AddRequestData("vnp_Version", _settings.Version);
            pay.AddRequestData("vnp_Command", _settings.Command);
            pay.AddRequestData("vnp_TmnCode", _settings.TmnCode);
            pay.AddRequestData("vnp_Amount", ((long)amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _settings.CurrCode);
            pay.AddRequestData("vnp_IpAddr", GetIpAddress(httpContext) ?? "127.0.0.1");
            pay.AddRequestData("vnp_Locale", _settings.Locale);
            //pay.AddRequestData("vnp_OrderInfo", cleanOrderInfo.Trim()); // <<-- SỬ DỤNG orderInfo đã được làm sạch
            pay.AddRequestData("vnp_OrderInfo", "Nang-cap-Premium");

            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", _settings.ReturnUrl);
            pay.AddRequestData("vnp_TxnRef", orderId);

            // --- Tạo URL thanh toán ---
            string paymentUrl = pay.CreateRequestUrl(_settings.BaseUrl, _settings.HashSecret);


            return paymentUrl;
        }

        // Hàm helper để loại bỏ dấu tiếng Việt
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            string normalizedString = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (char c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        // Infrastructure/Services/VnpayService.cs
        public bool ValidateSignature(IQueryCollection vnpayData, string inputHash)
        {
            var pay = new VnpayLibrary();
            // Truyền trực tiếp IQueryCollection vào hàm validate của VnpayLibrary
            return pay.ValidateSignature(inputHash, _settings.HashSecret, vnpayData);
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

        public Task<string> CreatePaymentLink(string type, VNPayRequest request, string returnUrl)
        {
            throw new NotImplementedException();
        }

        public Task<(bool? result, IDictionary<string, string> data)> QueryDR(string requestCode, string orderCode, DateTime transactionDate)
        {
            throw new NotImplementedException();
        }

        public Task<(string type, VNPayResponse response, string returnUrl)> ProcessCallBack()
        {
            throw new NotImplementedException();
        }
    }
}
