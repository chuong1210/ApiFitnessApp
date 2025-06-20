using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    // FitnessApp.Domain/Interfaces/IUnitOfWork.cs
  

    // Kế thừa IDisposable để đảm bảo DbContext được giải phóng đúng cách
    public interface IUnitOfWork : IDisposable
    {
        // --- Khai báo các Repository Properties ---
        // Mỗi thuộc tính sẽ cung cấp quyền truy cập vào một repository cụ thể.
        // Việc này giúp các lớp Application Layer chỉ cần inject IUnitOfWork
        // thay vì inject nhiều repository riêng lẻ.

        IUserRepository Users { get; }
        IWorkoutRepository Workouts { get; } // Giả sử bạn có interface này
        IWorkoutPlanRepository WorkoutPlans { get; } // Giả sử bạn có interface này
        IWorkoutSessionRepository WorkoutSessions { get; }
        IDailyActivityRepository DailyActivities { get; }
        IWorkoutPlanItemRepository WorkoutPlanItems { get; }

        IFoodItemRepository FoodItems { get; } // Giả sử bạn có interface này
        IMealLogRepository MealLogs { get; } // Giả sử bạn có interface này
        IScheduledMealRepository ScheduledMeals { get; } // Giả sử bạn có interface này
        ISleepLogRepository SleepLogs { get; } // Giả sử bạn có interface này
        IGoalRepository Goals { get; } // Giả sử bạn có interface này
                                       // ... Thêm các interface repository khác nếu cần
        INotificationRepository Notifications { get; } // Giả sử bạn có interface này
                                       // ... Thêm các interface repository khác nếu cần
        IPaymentTransactionRepository PaymentTransactions { get; }

        IHeartRateLogRepository HeartRateLogs { get; }
        ISleepScheduleRepository SleepSchedules { get; }

        // --- Phương thức Lưu thay đổi ---
        /// <summary>
        /// Lưu tất cả các thay đổi được thực hiện trong unit of work này vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Số lượng bản ghi trạng thái đã được ghi vào cơ sở dữ liệu.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // --- (Tùy chọn) Quản lý Transaction rõ ràng ---
        // Mặc dù SaveChangesAsync() của EF Core thường chạy trong transaction,
        // bạn có thể muốn quản lý transaction một cách rõ ràng hơn trong các kịch bản phức tạp.
        // Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        // Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        // Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
