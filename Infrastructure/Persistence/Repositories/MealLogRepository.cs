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

    public class MealLogRepository : IMealLogRepository
    {
        private readonly AppDbContext _context;

        public MealLogRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<MealLog?> GetByIdAsync(int logId, CancellationToken cancellationToken = default)
        {
            return await _context.MealLogs
                                 .Include(ml => ml.FoodItem) // Include food details
                                 .FirstOrDefaultAsync(ml => ml.LogId == logId, cancellationToken);
        }

        public async Task AddAsync(MealLog mealLog, CancellationToken cancellationToken = default)
        {
            await _context.MealLogs.AddAsync(mealLog, cancellationToken);
        }

        public async Task<IEnumerable<MealLog>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default)
        {
            return await _context.MealLogs
                                 .Where(ml => ml.UserId == userId && ml.Timestamp >= startDateTime && ml.Timestamp < endDateTime)
                                 .OrderByDescending(ml => ml.Timestamp) // Most recent first
                                 .Include(ml => ml.FoodItem) // Include food details
                                 .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MealLog>> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            var startDateTime = date.ToDateTime(TimeOnly.MinValue);
            var endDateTime = startDateTime.AddDays(1);
            return await GetByUserIdAndDateRangeAsync(userId, startDateTime, endDateTime, cancellationToken);
        }

        public void Update(MealLog mealLog)
        {
            _context.MealLogs.Update(mealLog);
        }

        public void Remove(MealLog mealLog)
        {
            _context.MealLogs.Remove(mealLog);
        }
      public IQueryable<MealLog> GetQueryable() => _context.MealLogs.AsNoTracking(); 
            public IQueryable<MealLog> GetQueryableByUserId(int userId) =>
          _context.MealLogs.Where(ml => ml.UserId == userId).AsNoTracking(); // AsNoTracking vì chỉ đọc
    }
}
