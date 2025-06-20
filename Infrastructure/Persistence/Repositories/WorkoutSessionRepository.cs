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
    public class WorkoutSessionRepository : IWorkoutSessionRepository
    {
        private readonly AppDbContext _context;

        public WorkoutSessionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<WorkoutSession?> GetByIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutSessions
                                 .Include(ws => ws.Plan) // Optional includes
                                 .Include(ws => ws.Workout)
                                 .FirstOrDefaultAsync(ws => ws.SessionId == sessionId, cancellationToken);
        }

        public async Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default)
        {
            await _context.WorkoutSessions.AddAsync(session, cancellationToken);
        }

        public async Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutSessions
                                 .Where(ws => ws.UserId == userId)
                                 .OrderByDescending(ws => ws.StartTime) // Most recent first
                                 .Include(ws => ws.Plan) // Optional includes
                                 .Include(ws => ws.Workout)
                                 .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkoutSession>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutSessions
                                .Where(ws => ws.UserId == userId && ws.StartTime >= startDateTime && ws.StartTime < endDateTime)
                                .OrderByDescending(ws => ws.StartTime)
                                .Include(ws => ws.Plan)
                                .Include(ws => ws.Workout)
                                .ToListAsync(cancellationToken);
        }

        public void Update(WorkoutSession session)
        {
            _context.WorkoutSessions.Update(session);
        }

        public void Remove(WorkoutSession session)
        {
            _context.WorkoutSessions.Remove(session);
        }


        public IQueryable<WorkoutSession> GetQueryable()
        {
            // Sử dụng AsNoTracking() vì phương thức này thường được dùng cho các truy vấn chỉ đọc
            // để xây dựng các query phức tạp ở tầng Application.
            return _context.WorkoutSessions.AsNoTracking();
        }
    }
    }
