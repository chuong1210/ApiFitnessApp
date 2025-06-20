using Domain.Entities;
using Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; // Thêm using này
using System.IO; // Thêm using này
using Domain.Interfaces;
namespace Infrastructure
{
    public class AppDbContext: DbContext
    {
        private readonly EntitySaveChangesInterceptor _saveChangesInterceptor;

   
        public AppDbContext(
        DbContextOptions options,
        EntitySaveChangesInterceptor saveChangesInterceptor)
        : base(options)
        {
            _saveChangesInterceptor = saveChangesInterceptor;


        }

        public DbSet<User> Users { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanItem> WorkoutPlanItems { get; set; }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<DailyActivity> DailyActivities { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<MealLog> MealLogs { get; set; }
        public DbSet<ScheduledMeal> ScheduledMeals { get; set; }
        public DbSet<SleepLog> SleepLogs { get; set; }
        public DbSet<Goal> Goals { get; set; }
        // Infrastructure/Persistence/AppDbContext.cs
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<WorkoutStep> WorkoutSteps { get; set; } // THÊM DÒNG NÀY
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<HeartRateLog> HeartRateLogs { get; set; } // <<--- THÊM DÒNG NÀY
        public DbSet<SleepSchedule> SleepSchedules { get; set; } // <<--- THÊM DÒNG NÀY



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from the current assembly (Infrastructure)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


            // --- ÁP DỤNG CONVENTION CHO CÁC TRƯỜNG AUDIT ---
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Kiểm tra xem entity có implement IAuditableEntity không
                if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Cấu hình cho CreatedAt và UpdatedAt
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IAuditableEntity.CreatedAt))
                        .HasColumnType("datetime2");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IAuditableEntity.UpdatedAt))
                        .HasColumnType("datetime2");

                    // Cấu hình cho CreatedBy và UpdatedBy
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IAuditableEntity.CreatedBy))
                        .HasMaxLength(256) // Độ dài phù hợp cho UserId/Username
                        .HasColumnType("nvarchar(256)"); // Đảm bảo kiểu nvarchar

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(IAuditableEntity.UpdatedBy))
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");
                }

                // --- (Tùy chọn) CONVENTION CHO CÁC THUỘC TÍNH KHÁC ---
                // Ví dụ: Tất cả các thuộc tính kiểu string có tên kết thúc bằng "Description"
                // sẽ có kiểu nvarchar(max)
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.Name.EndsWith("Description"))
                    {
                        // property.SetMaxLength(1000); // Giới hạn độ dài nếu cần
                        property.SetIsUnicode(true); // Đảm bảo là nvarchar
                                                     // Nếu muốn nvarchar(max) và EF Core không tự làm, có thể cần cách khác hoặc dùng HasColumnType
                    }

                    // Ví dụ: Tất cả các thuộc tính kiểu decimal sẽ có precision và scale mặc định
                    if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                    {
                        property.SetPrecision(18); // Tổng số chữ số
                        property.SetScale(2);    // Số chữ số sau dấu phẩy
                                                 // Hoặc dùng: property.SetColumnType("decimal(18,2)");
                    }
                }
            }
            // --- KẾT THÚC ÁP DỤNG CONVENTION ---

            base.OnModelCreating(modelBuilder);
        }

        // Optional: Override SaveChangesAsync for Auditing or Domain Events
        // public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        // {
        //     // Dispatch Domain Events before saving
        //     // Update Audit properties (CreatedAt, UpdatedAt)
        //     return await base.SaveChangesAsync(cancellationToken);
        // }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Order of the interceptors is important
            optionsBuilder.AddInterceptors(_saveChangesInterceptor);
        }
    }
}
