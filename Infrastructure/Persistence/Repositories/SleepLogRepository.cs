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

    public class SleepLogRepository : ISleepLogRepository
    {
        private readonly AppDbContext _context;

        public SleepLogRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SleepLog?> GetByIdAsync(int sleepLogId, CancellationToken cancellationToken = default)
        {
            return await _context.SleepLogs.FindAsync(new object[] { sleepLogId }, cancellationToken);
        }

        public async Task AddAsync(SleepLog sleepLog, CancellationToken cancellationToken = default)
        {
            await _context.SleepLogs.AddAsync(sleepLog, cancellationToken);
        }

        public async Task<IEnumerable<SleepLog>> GetByUserIdAndStartDateRangeAsync(int userId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default)
        {
            return await _context.SleepLogs
                                 .Where(sl => sl.UserId == userId && sl.StartTime >= startDateTime && sl.StartTime < endDateTime)
                                 .OrderByDescending(sl => sl.StartTime)
                                 .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<SleepLog>> GetByUserIdAndEndDateRangeAsync(int userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
        {
            var startOfDay = startDate.ToDateTime(TimeOnly.MinValue);
            var endOfRange = endDate.ToDateTime(TimeOnly.MinValue).AddDays(1); // Up to, but not including, the next day

            return await _context.SleepLogs
                                 .Where(sl => sl.UserId == userId && sl.EndTime >= startOfDay && sl.EndTime < endOfRange)
                                 .OrderByDescending(sl => sl.EndTime)
                                 .ToListAsync(cancellationToken);
        }

        public void Update(SleepLog sleepLog)
        {
            _context.SleepLogs.Update(sleepLog);
        }
        public IQueryable<SleepLog> GetAllQueryable()
        {
            // Sử dụng AsNoTracking() vì IQueryable thường dùng cho các truy vấn
            // phức tạp hơn mà không cần theo dõi thay đổi (cải thiện hiệu năng).
            // Nếu bạn cần tracking, bỏ AsNoTracking() đi.
            return _context.SleepLogs.AsNoTracking();
        }
        public void Remove(SleepLog sleepLog)
        {
            _context.SleepLogs.Remove(sleepLog);
        }
    }
    }
