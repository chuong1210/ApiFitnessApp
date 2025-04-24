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
        public int? UserId => int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) // Hoặc JwtRegisteredClaimNames.Sub
                                            ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)
                                ? userId
                                : null;
      
        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) // Hoặc JwtRegisteredClaimNames.Email
                                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Email);

        public string? Type => _httpContextAccessor.HttpContext?.User?.FindFirstValue(CONSTANT_CLAIM_TYPES.Type);


    }
}
