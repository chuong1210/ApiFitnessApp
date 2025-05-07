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
using System.Reflection;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly RedisSettings _redisSettings;

        private readonly ILogger<EmailService> _logger;
        //private readonly IConfiguration _config; cách 2 nếu ko dùng IOption
        private readonly string _templateFolderPath; // Đường dẫn đến thư mục chứa template


        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger, IConfiguration config, IOptions<RedisSettings> redisSettings)
        {
            _mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
            _logger = logger;
            _redisSettings = redisSettings.Value?? throw new ArgumentNullException(nameof(redisSettings));
            //_config = config;
            //string host = _configuration["MailSettings:Host"];
            //int port = int.Parse(_configuration["MailSettings:Port"]);
            //string username = _configuration["MailSettings:UserName"];
            //string password = _configuration["MailSettings:Password"];


            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var baseDirectory = AppContext.BaseDirectory;

            // Đường dẫn này sẽ trỏ đến thư mục bin/Debug hoặc bin/Release của project Infrastructure
            _templateFolderPath = Path.Combine(baseDirectory ?? string.Empty, "Assets", "EmailTemplates");

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
                // Màu sắc và Font (Bạn có thể định nghĩa ở đây hoặc dùng biến)
                string primaryColor = "#6A1B9A"; // Màu tím đậm (ví dụ)
                string secondaryColor = "#F3E5F5"; // Màu tím nhạt
                string textColor = "#333333";
                string otpColor = "#D81B60"; // Màu hồng đậm cho OTP
                string fontFamily = "Arial, sans-serif";

                builder.HtmlBody = $@"
<!DOCTYPE html>
<html lang=""vi"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{mail.subject}</title>
    <style>
        /* Một số style cơ bản cho các client hỗ trợ style tag */
        body {{ margin: 0; padding: 0; font-family: {fontFamily}; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }}
        .header {{ background-color: {primaryColor}; padding: 30px 20px; text-align: center; }}
        .header h1 {{ color: #ffffff; margin: 0; font-size: 28px; }}
        .content {{ padding: 30px 40px; color: {textColor}; line-height: 1.6; }}
        .otp-code {{ background-color: {secondaryColor}; border: 1px dashed {primaryColor}; border-radius: 5px; padding: 15px 20px; text-align: center; margin: 25px 0; }}
        .otp-code h2 {{ color: {otpColor}; margin: 0; font-size: 36px; letter-spacing: 5px; font-weight: bold; }}
        .footer {{ background-color: #eeeeee; padding: 20px; text-align: center; font-size: 12px; color: #777777; }}
        .button {{ display: inline-block; background-color: {primaryColor}; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; margin-top: 20px; }}
         /* Responsive */
        @media screen and (max-width: 600px) {{
            .content {{ padding: 20px; }}
            .header h1 {{ font-size: 24px; }}
            .otp-code h2 {{ font-size: 30px; }}
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; font-family: {fontFamily}; background-color: #f4f4f4;"">
    <table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
        <tr>
            <td style=""padding: 20px 0;"">
                <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td align=""center"" bgcolor=""{primaryColor}"" style=""padding: 30px 20px; text-align: center;"">
                            <h1 style=""color: #ffffff; margin: 0; font-size: 28px;"">Xác Thực Tài Khoản</h1>
                            <!-- Optional: Thêm logo của bạn ở đây -->
                            <!-- <img src=""your_logo_url"" alt=""Your App Logo"" width=""150"" style=""display: block; margin: 10px auto 0;""> -->
                        </td>
                    </tr>
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 30px 40px; color: {textColor}; line-height: 1.6; font-size: 16px;"">
                            <p>Chào bạn,</p>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại <strong>{_mailSettings.SenderName}</strong>. Để hoàn tất quá trình đăng ký và đảm bảo an toàn cho tài khoản, vui lòng sử dụng mã xác thực (OTP) dưới đây:</p>

                            <!-- OTP Code Box -->
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin: 25px 0;"">
                                <tr>
                                    <td align=""center"">
                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                                            <tr>
                                                <td bgcolor=""{secondaryColor}"" style=""border: 1px dashed {primaryColor}; border-radius: 5px; padding: 15px 30px; text-align: center;"">
                                                    <h2 style=""color: {otpColor}; margin: 0; font-size: 36px; letter-spacing: 5px; font-weight: bold;"">{mail.otpCode}</h2>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>

                            <p style=""text-align: center; font-size: 14px; color: #555555;"">Mã OTP này sẽ hết hạn sau <strong>{_redisSettings.OtpExpiryMinutes} phút</strong>.</p>
                            <p>Vui lòng nhập mã này vào ứng dụng để xác thực địa chỉ email của bạn. Tuyệt đối không chia sẻ mã này với bất kỳ ai.</p>
                            <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này một cách an toàn.</p>
                            <!-- Optional: Add a button if needed -->
                            <!--
                            <p style=""text-align: center;"">
                                <a href=""your_app_link_or_website"" target=""_blank"" style=""display: inline-block; background-color: {primaryColor}; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; margin-top: 20px;"">
                                    Mở ứng dụng
                                </a>
                            </p>
                            -->
                        </td>
                    </tr>
                    <!-- Footer -->
                    <tr>
                        <td bgcolor=""#eeeeee"" style=""padding: 20px; text-align: center; font-size: 12px; color: #777777;"">
                            <p style=""margin: 0;"">Bạn nhận được email này vì đã đăng ký tài khoản tại {_mailSettings.SenderName}.</p>
                            <p style=""margin: 5px 0 0;"">© {DateTime.UtcNow.Year} {_mailSettings.SenderName}. All rights reserved.</p>
                            <!-- Optional: Add social media links or address -->
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";

                // --- Kết thúc tạo nội dung ---


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


        public async Task SendPremiumUpgradeEmailAsync(PremiumUpgradeEmailDto mail)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(mail.toEmail));
                var Subject = "🎉 Welcome to Fitness App Premium!";

                // --- Đọc và xử lý template HTML ---
                string templatePath = Path.Combine(_templateFolderPath, "PremiumUpgradeConfirmation.html");
                if (!File.Exists(templatePath))
                {
                    _logger.LogError("Premium upgrade email template not found at: {Path}", templatePath);
                    // Gửi email text đơn giản thay thế hoặc báo lỗi
                    await SendPlainTextEmailFallback(mail.toEmail, Subject, $"Dear {mail.userName},\n\nYour account has been successfully upgraded to Premium!\n\nThank you,\nThe Fitness App Team");
                    return; // Thoát khỏi phương thức sau khi gửi fallback
                }

                string htmlBody = await File.ReadAllTextAsync(templatePath);

                // Thay thế placeholders
                htmlBody = htmlBody.Replace("{{UserName}}", mail.userName);
                htmlBody = htmlBody.Replace("{{CurrentYear}}", DateTime.UtcNow.Year.ToString());
                // htmlBody = htmlBody.Replace("URL_APP_LOGO", "URL_THUC_TE_CUA_LOGO"); // Thay thế URL logo nếu dùng URL
                // htmlBody = htmlBody.Replace("URL_GO_TO_APP", "URL_TRANG_CHU_APP");
                // htmlBody = htmlBody.Replace("URL_UNSUBSCRIBE", "URL_HUY_DANG_KY_EMAIL");
                // htmlBody = htmlBody.Replace("URL_PRIVACY_POLICY", "URL_CHINH_SACH_BAO_MAT");

                var builder = new BodyBuilder { HtmlBody = htmlBody };

                // (Tùy chọn) Nhúng ảnh trực tiếp vào email (nếu ảnh nhỏ và ít)
                // var logoPath = Path.Combine(_templateFolderPath, "logo.png");
                // if(File.Exists(logoPath)) {
                //     var logoImage = builder.LinkedResources.Add(logoPath);
                //     logoImage.ContentId = MimeUtils.GenerateMessageId(); // Tạo Content-ID
                //     // Sửa thẻ img trong HTML: <img src="cid:{logoImage.ContentId}" />
                //     htmlBody = htmlBody.Replace("URL_LOGO_CUA_BAN", $"cid:{logoImage.ContentId}");
                //     builder.HtmlBody = htmlBody; // Cập nhật lại body
                // }
                email.Body = builder.ToMessageBody();

                email.Subject = Subject;


                // --- Gửi Email (giống SendOtpEmailAsync) ---
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                await smtp.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                _logger.LogInformation("Premium upgrade confirmation email sent successfully to {Recipient}.",mail. toEmail);

            }
            catch (FileNotFoundException fnfEx)
            {
                _logger.LogError(fnfEx, "Email template file not found during premium upgrade email sending to {Recipient}.",mail. toEmail);
                // Có thể thử gửi fallback ở đây
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send premium upgrade email to {Recipient}.", mail.toEmail);
            }
        }

        // Phương thức gửi email text dự phòng
        private async Task SendPlainTextEmailFallback(string toEmail, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                await smtp.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                _logger.LogWarning("Sent plain text fallback email to {Recipient} because template was missing.", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send plain text fallback email to {Recipient}.", toEmail);
            }
        }


    }
}
