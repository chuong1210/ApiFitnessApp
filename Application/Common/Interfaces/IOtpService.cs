using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{

    public interface IOtpService
    {
        string GenerateOtp(int length = 4);
        Task StoreOtpAsync(string key, string otp);
        Task<string?> GetOtpAsync(string key);
        Task<bool> VerifyOtpAsync(string key, string providedOtp);
        Task RemoveOtpAsync(string key);
    }
}
