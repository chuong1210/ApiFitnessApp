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

    public class DailyActivityRepository : IDailyActivityRepository
    {
        private readonly AppDbContext _context;

        public DailyActivityRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DailyActivity?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.DailyActivities
                                 .FirstOrDefaultAsync(da => da.UserId == userId && da.Date == date, cancellationToken);
        }

        public async Task AddAsync(DailyActivity activity, CancellationToken cancellationToken = default)
        {
            await _context.DailyActivities.AddAsync(activity, cancellationToken);
        }

        public void Update(DailyActivity activity)
        {
            _context.DailyActivities.Update(activity);
        }

        public async Task<IEnumerable<DailyActivity>> GetByUserIdAndDateRangeAsync(int userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            return await _context.DailyActivities
                                 .Where(da => da.UserId == userId && da.Date >= startDate && da.Date <= endDate)
                                 .OrderBy(da => da.Date) // Order chronologically
                                 .ToListAsync(cancellationToken);
        }

        public void Remove(DailyActivity activity)
        {
            _context.DailyActivities.Remove(activity);
        }

        public IQueryable<DailyActivity> GetAllQueryable()
        {
            // Sử dụng AsNoTracking() vì phương thức này được thiết kế cho các truy vấn
            // chỉ đọc (read-only) và không cần EF Core theo dõi các thay đổi.
            return _context.DailyActivities.AsNoTracking();
        }
    }
    }
