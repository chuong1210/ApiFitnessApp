using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
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
            // Configure DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection"); // Get from appsettings.json
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString, builder =>
                {
                    builder.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                    //builder.EnableRetryOnFailure();
                })); // Use SQLite provider

            // Register Repositories
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IDailyActivityRepository, DailyActivityRepository>();
            //services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>(); // Add this
            //                                                                           // ... Register other repositories
            //                                                                           // services.AddScoped<IUnitOfWork, UnitOfWork>(); // If using Unit of Work pattern

            //// Register other Infrastructure services
            //services.AddSingleton<IDateTimeProvider, DateTimeProvider>(); // Example
            services.AddSingleton<IDateTimeService, DateTimeService>();

            return services;
        }
    }
}
