using Application.Common.Interfaces;
using Infrastructure.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{

    public class RedisOtpService : IOtpService
    {
        private readonly IDistributedCache _cache; // Inject IDistributedCache
        private readonly RedisSettings _redisSettings;
        private readonly IConfiguration _configuration;

        private readonly ILogger<RedisOtpService> _logger;
        private readonly Random _random = new Random();


        public RedisOtpService(
            IDistributedCache cache,
            IOptions<RedisSettings> redisSettings,
            ILogger<RedisOtpService> logger,
            IConfiguration configuration)
        {
            _cache = cache;
            _redisSettings = redisSettings.Value;
            _logger = logger;
            _configuration = configuration;
        }

        public string GenerateOtp( int length = 4) // Tạo OTP 4 chữ số
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task StoreOtpAsync(string key, string otp)
        {
            try
            {
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(_redisSettings.OtpExpiryMinutes));
                var redisKey = $"otp:{key}"; // Prefix để tránh trùng key khác
                await _cache.SetStringAsync(redisKey, otp, options);
                _logger.LogInformation("Stored OTP for key {Key} with expiry {ExpiryMinutes} mins.", redisKey, _redisSettings.OtpExpiryMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing OTP for key {Key} in Redis.", $"otp:{key}");
                // Cần có cơ chế xử lý lỗi hoặc báo lỗi phù hợp
                throw; // Ném lại lỗi để Handler biết và xử lý
            }
        }

        public async Task<string?> GetOtpAsync(string key)
        {
            try
            {
                var redisKey = $"otp:{key}";
                var otp = await _cache.GetStringAsync(redisKey);
                if (otp != null)
                {
                    _logger.LogDebug("Retrieved OTP for key {Key}.", redisKey);
                }
                else
                {
                    _logger.LogDebug("No OTP found or expired for key {Key}.", redisKey);
                }
                return otp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving OTP for key {Key} from Redis.", $"otp:{key}");
                return null; // Trả về null nếu có lỗi đọc cache
            }
        }

        public async Task<bool> VerifyOtpAsync(string key, string providedOtp)
        {
            var storedOtp = await GetOtpAsync(key);
            if (storedOtp != null && storedOtp == providedOtp)
            {
                _logger.LogInformation("OTP verified successfully for key {Key}.", $"otp:{key}");
                // Không xóa OTP ở đây, để Handler quyết định khi nào xóa
                return true;
            }
            _logger.LogWarning("OTP verification failed for key {Key}. Provided: '{ProvidedOtp}', Stored: '{StoredOtp}'", $"otp:{key}", providedOtp, storedOtp ?? "NULL/Expired");
            return false;
        }

        public async Task RemoveOtpAsync(string key)
        {
            try
            {
                var redisKey = $"otp:{key}";
                await _cache.RemoveAsync(redisKey);
                _logger.LogInformation("Removed OTP for key {Key}.", redisKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing OTP for key {Key} from Redis.", $"otp:{key}");
                // Ghi log nhưng không cần ném lỗi
            }
        }


    }
}
