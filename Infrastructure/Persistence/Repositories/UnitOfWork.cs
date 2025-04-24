using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed = false; // Để quản lý việc dispose

        // Các backing fields cho repositories (sử dụng lazy loading)
        private IUserRepository? _userRepository;
        private IWorkoutRepository? _workoutRepository;
        private IWorkoutPlanRepository? _workoutPlanRepository;
        private IWorkoutPlanItemRepository? _workoutPlanItemRepository;

        private IWorkoutSessionRepository? _workoutSessionRepository;
        private IDailyActivityRepository? _dailyActivityRepository;
        private IFoodItemRepository? _foodItemRepository;
        private IMealLogRepository? _mealLogRepository;
        private IScheduledMealRepository? _scheduledMealRepository;
        private ISleepLogRepository? _sleepLogRepositssory;
        private IGoalRepository? _goalRepository;
        private IPaymentTransactionRepository? _paymentTransactionRepository;
        // ... thêm các backing fields khác

        // Inject AppDbContext
        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Implement các Repository Properties (Lazy Initialization)
        // Điều này đảm bảo repository chỉ được tạo khi nó thực sự được yêu cầu.
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IWorkoutRepository Workouts => _workoutRepository ??= new WorkoutRepository(_context); // Cần tạo lớp WorkoutRepository
        public IWorkoutPlanRepository WorkoutPlans => _workoutPlanRepository ??= new WorkoutPlanRepository(_context); // Cần tạo lớp WorkoutPlanRepository
        public IWorkoutPlanItemRepository WorkoutPlanItems => _workoutPlanItemRepository ??= new WorkoutPlanItemRepository(_context); // Cần tạo lớp WorkoutPlanRepository

        public IWorkoutSessionRepository WorkoutSessions => _workoutSessionRepository ??= new WorkoutSessionRepository(_context);
        public IDailyActivityRepository DailyActivities => _dailyActivityRepository ??= new DailyActivityRepository(_context);
        public IFoodItemRepository FoodItems => _foodItemRepository ??= new FoodItemRepository(_context); // Cần tạo lớp FoodItemRepository
        public IMealLogRepository MealLogs => _mealLogRepository ??= new MealLogRepository(_context); // Cần tạo lớp MealLogRepository
        public IScheduledMealRepository ScheduledMeals => _scheduledMealRepository ??= new ScheduledMealRepository(_context); // Cần tạo lớp ScheduledMealRepository
        public ISleepLogRepository SleepLogs => _sleepLogRepositssory ??= new SleepLogRepository(_context); // Cần tạo lớp SleepLogRepository
        public IGoalRepository Goals => _goalRepository ??= new GoalRepository(_context); // Cần tạo lớp GoalRepository
                                                                                          // ... implement các properties khác
        public IPaymentTransactionRepository PaymentTransactions =>
              _paymentTransactionRepository ??= new PaymentTransactionRepository(_context);

        // Implement SaveChangesAsync
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Tại đây có thể thêm logic tiền xử lý trước khi lưu (ví dụ: cập nhật các trường audit)
            // hoặc gọi các interceptor nếu có.
            return await _context.SaveChangesAsync(cancellationToken);
        }

        // Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Ngăn GC gọi Finalizer nếu Dispose đã được gọi
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Giải phóng các tài nguyên managed ở đây (ví dụ: DbContext)
                    _context.Dispose();
                }

                // Giải phóng các tài nguyên unmanaged (nếu có)

                _disposed = true;
            }
        }

        // Optional: Implement transaction methods if defined in the interface
        // public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) { ... _context.Database.BeginTransactionAsync ... }
        // public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) { ... _context.Database.CommitTransactionAsync ... }
        // public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) { ... _context.Database.RollbackTransactionAsync ... }
    }
}
