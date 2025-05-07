using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Dtos;
namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        //Task SendOtpEmailAsync(string toEmail, string subject, string otpCode);
        Task SendOtpEmailAsync(EmailDto mail);

        Task SendPremiumUpgradeEmailAsync(PremiumUpgradeEmailDto mail); 

    }
}
