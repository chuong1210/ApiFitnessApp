using Application.Common.Interfaces;
using FitnessApp.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FitnessApp.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        //public int? UserId
        //{
        //    get
        //    {
        //        string userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(CONSTANT_CLAIM_TYPES.Uid);
        //        if (userIdString != null && int.TryParse(userIdString, out int userId))
        //        {
        //            return userId;
        //        }
        //        return null;
        //    }
        //}



        // Lấy UserId từ claim 'sub' (Subject) của token

        public int? UserId
        {
            get
            {
                // Ưu tiên đọc từ header do Gateway chuyển tiếp
                var userIdFromHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault();
                if (int.TryParse(userIdFromHeader, out var id))
                {
                    return id;
                }
                // Nếu không có header, thử đọc từ claim (phòng trường hợp gọi trực tiếp service không qua Gateway)
                return int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                    _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var claimId)
                                ? claimId
                                : null;
            }
        }
        //public int? UserId => int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) // Hoặc JwtRegisteredClaimNames.Sub
        //                                    ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)
        //                        ? userId
        //                        : int.Parse(_httpContextAccessor.HttpContext?.Request.Headers["X-User-Id"].FirstOrDefault());

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) // Hoặc JwtRegisteredClaimNames.Email
                                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Email);

        public string? Type => _httpContextAccessor.HttpContext?.User?.FindFirstValue(CONSTANT_CLAIM_TYPES.Type);


    }
}
