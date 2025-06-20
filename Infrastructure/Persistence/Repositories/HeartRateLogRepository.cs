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

    public class HeartRateLogRepository : IHeartRateLogRepository
    {
        private readonly AppDbContext _context;

        public HeartRateLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<HeartRateLog> GetQueryable()
        {
            return _context.HeartRateLogs.AsNoTracking();
        }

        public async Task AddAsync(HeartRateLog log, CancellationToken cancellationToken = default)
        {
            await _context.HeartRateLogs.AddAsync(log, cancellationToken);
        }
    }
}
