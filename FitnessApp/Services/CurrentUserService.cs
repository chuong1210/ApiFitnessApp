using Application.Common.Interfaces;
using FitnessApp.Constants;
using System.Security.Claims;

namespace FitnessApp.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public int? UserId
        {
            get
            {
                string userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(CONSTANT_CLAIM_TYPES.Uid);
                if (userIdString != null && int.TryParse(userIdString, out int userId))
                {
                    return userId;
                }
                return null;
            }
        }

        public string? Type => _httpContextAccessor.HttpContext?.User?.FindFirstValue(CONSTANT_CLAIM_TYPES.Type);


    }
}
