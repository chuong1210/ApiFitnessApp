using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{

    public class ScheduledMealRepository : IScheduledMealRepository
    {
        private readonly AppDbContext _context;

        public ScheduledMealRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ScheduledMeal?> GetByIdAsync(int scheduleId, CancellationToken cancellationToken = default)
        {
            return await _context.ScheduledMeals
                                 .Include(sm => sm.PlannedFoodItem) // Include food details if linked
                                 .FirstOrDefaultAsync(sm => sm.ScheduleId == scheduleId, cancellationToken);
        }

        public async Task AddAsync(ScheduledMeal scheduledMeal, CancellationToken cancellationToken = default)
        {
            await _context.ScheduledMeals.AddAsync(scheduledMeal, cancellationToken);
        }

        public async Task<IEnumerable<ScheduledMeal>> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.ScheduledMeals
                                 .Where(sm => sm.UserId == userId && sm.Date == date)
                                 .OrderBy(sm => sm.MealType) // Order by meal type (Breakfast, Lunch, etc.)
                                 .Include(sm => sm.PlannedFoodItem)
                                 .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<ScheduledMeal>> GetByUserIdAndDateRangeAsync(int userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            return await _context.ScheduledMeals
                                 .Where(sm => sm.UserId == userId && sm.Date >= startDate && sm.Date <= endDate)
                                 .OrderBy(sm => sm.Date).ThenBy(sm => sm.MealType)
                                 .Include(sm => sm.PlannedFoodItem)
                                 .ToListAsync(cancellationToken);
        }

        public IQueryable<ScheduledMeal> GetQueryableByUserIdAndDate(int userId, DateOnly date)
        {
            return _context.ScheduledMeals
                           .Where(sm => sm.UserId == userId && sm.Date == date)
                           .AsNoTracking(); // Quan trọng: Dùng AsNoTracking() cho các query chỉ đọc để cải thiện hiệu năng
                                            // EF Core sẽ không theo dõi thay đổi trên các entity này.
        }

        public IQueryable<ScheduledMeal> GetQueryableByUserIdAndDateRange(int userId, DateOnly startDate, DateOnly endDate)
        {
            return _context.ScheduledMeals
                           .Where(sm => sm.UserId == userId && sm.Date >= startDate && sm.Date <= endDate)
                           .AsNoTracking();
        }
        public void Update(ScheduledMeal scheduledMeal)
        {
            _context.ScheduledMeals.Update(scheduledMeal);
        }

        public void Remove(ScheduledMeal scheduledMeal)
        {
            _context.ScheduledMeals.Remove(scheduledMeal);
        }
    }
    }
