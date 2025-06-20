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

    public class SleepScheduleRepository : ISleepScheduleRepository
    {
        private readonly AppDbContext _context;

        public SleepScheduleRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SleepSchedule?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.SleepSchedules
                                 .AsNoTracking() // Dùng AsNoTracking cho query chỉ đọc
                                 .FirstOrDefaultAsync(s => s.UserId == userId && s.ScheduleDate == date, cancellationToken);
        }

        public async Task<SleepSchedule?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.SleepSchedules.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(SleepSchedule schedule, CancellationToken cancellationToken = default)
        {
            await _context.SleepSchedules.AddAsync(schedule, cancellationToken);
        }

        public void Update(SleepSchedule schedule)
        {
            _context.SleepSchedules.Update(schedule);
        }

        public void Remove(SleepSchedule schedule)
        {
            _context.SleepSchedules.Remove(schedule);
        }

    }

}