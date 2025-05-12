using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Services;
namespace Infrastructure.Interceptors
{
    public class EntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTimeService _dateTime;

        public EntitySaveChangesInterceptor(ICurrentUserService currentUserService, IDateTimeService dateTime)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if (context is null)
                return;

            if (context == null) return;

            // Lấy UserId hoặc Username của người dùng hiện tại
            // Nếu không có người dùng đăng nhập (ví dụ: background job), có thể để null hoặc một giá trị mặc định
            var currentUserIdString = _currentUserService.UserId?.ToString(); // Lấy UserId dạng string
                                                                              // Hoặc: var currentUsername = _currentUserService.Username; (nếu bạn có Username trong ICurrentUserService)

            var utcNow = _dateTime.UtcNow; // Lấy thời gian UTC hiện tại từ service

            // Duyệt qua tất cả các entity đang được theo dõi bởi DbContext
            foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>()) // Chỉ xử lý các entity implement IAuditableEntity
            {
                // Nếu entity được thêm mới
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = currentUserIdString; // Gán người tạo
                    entry.Entity.CreatedAt = utcNow;         // Gán thời gian tạo
                                                             // UpdatedAt và UpdatedBy cũng có thể được gán giá trị giống Created khi tạo mới
                    entry.Entity.UpdatedBy = currentUserIdString;
                    entry.Entity.UpdatedAt = utcNow;
                }
                // Nếu entity được cập nhật
                else if (entry.State == EntityState.Modified || // Entity được sửa đổi tường minh
                         (entry.State == EntityState.Unchanged && entry.Properties.Any(p => p.IsModified))) // Hoặc một thuộc tính của nó bị sửa đổi (ít xảy ra nếu bạn dùng Update())
                {
                    entry.Entity.UpdatedBy = currentUserIdString; // Gán người cập nhật
                    entry.Entity.UpdatedAt = utcNow;         // Gán thời gian cập nhật

                    // (Tùy chọn) Không cho phép thay đổi CreatedAt và CreatedBy khi update
                    // entry.Property(e => e.CreatedAt).IsModified = false;
                    // entry.Property(e => e.CreatedBy).IsModified = false;
                }
            }

        }
    }
}
