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
            //SQLitePCL.Batteries.Init(); // Hoặc SQLitePCL.Batteries_V2.Init(); tùy thuộc vào gói bạn dùng


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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply all configurations from the current assembly (Infrastructure)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


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
