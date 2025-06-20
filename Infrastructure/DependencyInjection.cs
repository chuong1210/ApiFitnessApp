using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VNPay.NetCore;

namespace Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var redisConfig = configuration.GetSection("RedisSettings");

            services.AddScoped<EntitySaveChangesInterceptor>();

            // Configure DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection"); // Get from appsettings.json
            services.AddDbContext<AppDbContext>((sp, options) =>
               {
                   var interceptor = sp.GetRequiredService<EntitySaveChangesInterceptor>();

                   options.UseSqlServer(connectionString, builder =>
                {

                    builder.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                    builder.EnableRetryOnFailure();

                }
                )
               .AddInterceptors(interceptor); 

                   
               });
                
                
                
            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDailyActivityRepository, DailyActivityRepository>();
            services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>(); // Add this
                                                                                       // Infrastructure/DependencyInjection.cs
            services.AddScoped<ISleepAutoLogService, SleepAutoLogService>();                                     //                                
                                                                                                                 // ... Register other repositories
            services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>(); // Add this

            services.AddScoped<IWorkoutRepository, WorkoutRepository>(); // Add this
            services.AddScoped<IWorkoutPlanItemRepository, WorkoutPlanItemRepository>(); // Add this
            services.AddScoped<IHeartRateLogRepository, HeartRateLogRepository>(); // Add this
            services.AddScoped<INotificationRepository, NotificationRepository>(); // Add this
            services.AddScoped<IGoalRepository, GoalRepository>(); // Add this
            services.AddScoped<IDailyActivityRepository, DailyActivityRepository>(); // Add this
            services.AddScoped<IFoodItemRepository, FoodItemRepository>(); // Add this


            services.AddScoped<INotificationService, NotificationService>(); // Add this

            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<ICloudinaryService, CloudinaryService>(); // Scoped hoặc Transient đều ổn

            //// Register other Infrastructure services
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddScoped<AppDbContextInitialiser>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();


            services.Configure<VnpaySettings>(configuration.GetSection("Vnpay"));

            // Đăng ký VnpayService
            services.AddScoped<IVnpayService, VnpayService>(); // Scoped 
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings"));
            services.AddScoped<IEmailService, EmailService>();


            services.Configure<RedisSettings>(configuration.GetSection("RedisSettings")); // Vẫn giữ để inject vào OtpService

            // Lấy cấu hình Redis để xây dựng ConnectionString
            var redisHost = configuration.GetValue<string>("RedisSettings:Host");
            var redisPort = configuration.GetValue<int?>("RedisSettings:Port"); // Dùng int? để kiểm tra null
            if (string.IsNullOrEmpty(redisHost) || !redisPort.HasValue)
            {
                throw new InvalidOperationException("Redis Host or Port is not configured correctly in RedisSettings section.");
            }

            // Tạo ConnectionString hoàn chỉnh
            var redisConnectionString = $"{redisHost}:{redisPort.Value}";

         
            //services.AddSingleton<IConnectionMultiplexer>(provider =>
            //{

            //    var configuration = ConfigurationOptions.Parse($"localhost:{redisConfig["Port"]}", true); 
            //    return ConnectionMultiplexer.Connect(redisConnectionString);
            //});

            services.AddStackExchangeRedisCache(options =>
            {
                // Lấy ConnectionString từ cấu hình
                options.Configuration = redisConnectionString
                   ?? throw new InvalidOperationException("Redis ConnectionString is not configured in RedisSettings section."); // Ném lỗi nếu không có cấu hình

                options.InstanceName = "fitness_app_"; // Optional: prefix cho keys nếu dùng chung Redis server
            });
            // -----
            services.AddScoped<IOtpService, RedisOtpService>();

            return services;
        }
    }
}
