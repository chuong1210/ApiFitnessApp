using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using FitnessApp.Constants;

namespace FitnessApp.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IMapper _mapper;

        public PermissionService(IMapper pMapper)
        {
            _mapper = pMapper;
        }
        public List<string> GetPermissionsForUser(User user)
        {
            var permissions = new List<string>();

            // Quyền cơ bản cho mọi user đã đăng nhập
            permissions.Add(PERMISSIONS.ViewProfile);
            permissions.Add(PERMISSIONS.EditProfile);
            permissions.Add(PERMISSIONS.LogWorkout); // Ví dụ
            permissions.Add(PERMISSIONS.ViewStandardDashboard); // User thường cũng xem được dashboard thường

            // Quyền bổ sung cho Premium user
            if (user.IsPremium)
            {
                permissions.Add(PERMISSIONS.ViewPremiumDashboard);
                permissions.Add(PERMISSIONS.AccessPremiumPlans);
                permissions.Add(PERMISSIONS.GenerateReports);
                permissions.Add(PERMISSIONS.AdvancedNutritionTracking);
                // Premium user cũng có các quyền của standard user,
                // nên không cần remove các quyền đã add ở trên.
            }

            // Có thể thêm logic đọc quyền từ Role hoặc bảng UserPermissions ở đây nếu phức tạp hơn

            return permissions.Distinct().ToList(); // Đảm bảo không có quyền trùng lặp
        }
        public async Task Create(List<string> pPermissions)
        {
            //foreach (var permission in pPermissions)
            //{
            //    var per = await _context.Permissions
            //        .FirstOrDefaultAsync(x => x.Name == permission);

            //    if (per == null)
            //    {
            //        var newPer = new Permission { Name = permission };
            //        var newPermission = await _context.Permissions.AddAsync(newPer);
            //        await _context.SaveChangesAsync(default(CancellationToken));
            //    }
            //}
        }

  
    }
}
