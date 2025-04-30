using Application.Common.Dtos;
using Application.Common.Interfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp; // MailKit
using MailKit.Security; // MailKit
using MimeKit; // MimeKit
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;
       //private readonly IConfiguration _config; cách 2 nếu ko dùng IOption

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger,IConfiguration config)
        {
            _mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
            _logger = logger;
            //_config = config;
            //string host = _configuration["MailSettings:Host"];
            //int port = int.Parse(_configuration["MailSettings:Port"]);
            //string username = _configuration["MailSettings:UserName"];
            //string password = _configuration["MailSettings:Password"];
        }

        public async Task SendOtpEmailAsync(EmailDto mail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(mail.toEmail));
                email.Subject = mail.subject;

                // --- Tạo nội dung email (HTML hoặc Text) ---
                var builder = new BodyBuilder();
                builder.HtmlBody = $@"
                <h1>Xác thực tài khoản</h1>
                <p>Cảm ơn bạn đã đăng ký. Mã OTP của bạn là:</p>
                <h2 style='color: blue;'>{mail.otpCode}</h2>
                <p>Mã này sẽ hết hạn sau vài phút. Vui lòng không chia sẻ mã này.</p>
                <p>Trân trọng,<br/>Đội ngũ {_mailSettings.SenderName}</p>";
                // Hoặc dùng builder.TextBody cho email dạng text thuần túy

                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                _logger.LogInformation("Connecting to SMTP server {Host}:{Port}", _mailSettings.SmtpHost, _mailSettings.SmtpPort);

                // Kết nối an toàn (STARTTLS nếu cổng 587, SSL/TLS nếu cổng 465)
                await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable); // Tự động chọn StartTls hoặc SslOnConnect

                _logger.LogInformation("Authenticating SMTP server...");
                await smtp.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);

                _logger.LogInformation("Sending OTP email to {Recipient}", mail.toEmail);
                await smtp.SendAsync(email);

                _logger.LogInformation("OTP email sent successfully to {Recipient}.", mail.toEmail);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Recipient}.", mail.toEmail);
                // Không nên ném exception ra ngoài để không làm gián đoạn luồng đăng ký
                // Chỉ ghi log lại để điều tra
            }
        }
    }
}
