using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<EntitySaveChangesInterceptor>();

            // Configure DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection"); // Get from appsettings.json
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                    //builder.EnableRetryOnFailure();
                })); // Use SQLite provider

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IDailyActivityRepository, DailyActivityRepository>();
            //services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>(); // Add this
            //                                                                           // ... Register other repositories
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<ICloudinaryService, CloudinaryService>(); // Scoped hoặc Transient đều ổn

            //// Register other Infrastructure services
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddScoped<AppDbContextInitialiser>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVnpayService, VnpayService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();


            services.Configure<VnpaySettings>(configuration.GetSection("Vnpay"));

            // Đăng ký VnpayService
            services.AddScoped<IVnpayService, VnpayService>(); // Scoped 
            return services;
        }
    }
}
