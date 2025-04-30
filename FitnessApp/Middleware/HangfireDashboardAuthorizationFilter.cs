using Hangfire.Dashboard;

namespace FitnessApp.Middleware
{
    // --- Lớp Filter Authorization cho Hangfire Dashboard (Đặt ở đâu đó phù hợp) ---
    public class HangfireDashboardAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
    {
        public bool Authorize(Hangfire.Dashboard.DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Ví dụ: Chỉ cho phép user đã đăng nhập và có claim 'Admin' hoặc permission cụ thể
            // return httpContext.User?.Identity?.IsAuthenticated == true &&
            //        httpContext.User.IsInRole("Admin"); // Hoặc kiểm tra claim/permission
            // Hoặc đơn giản là luôn cho phép trong môi trường Development
#if DEBUG
            return true;
#else
        // Logic kiểm tra quyền chặt chẽ hơn cho Production
        return httpContext.User?.Identity?.IsAuthenticated == true &&
               httpContext.User.HasClaim(c => c.Type == "permission" && c.Value == "Permissions.Hangfire.View"); // Ví dụ permission
#endif
        }
    }
    }
