using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly AppDbContext _context;

        public GoalRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Goal?> GetByIdAsync(int goalId, CancellationToken cancellationToken = default)
        {
            return await _context.Goals.FindAsync(new object[] { goalId }, cancellationToken);
        }

        public async Task AddAsync(Goal goal, CancellationToken cancellationToken = default)
        {
            await _context.Goals.AddAsync(goal, cancellationToken);
        }

        public async Task<IEnumerable<Goal>> GetActiveGoalsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Goals
                                 .Where(g => g.UserId == userId && g.IsActive)
                                 .OrderBy(g => g.GoalType) // Optional ordering
                                 .ToListAsync(cancellationToken);
        }

        public async Task<Goal?> GetActiveGoalByTypeAsync(int userId, GoalType goalType, CancellationToken cancellationToken = default)
        {
            return await _context.Goals
                                 .FirstOrDefaultAsync(g => g.UserId == userId && g.IsActive && g.GoalType == goalType, cancellationToken);
        }

        public async Task<IEnumerable<Goal>> GetAllGoalsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Goals
                                 .Where(g => g.UserId == userId)
                                 .OrderByDescending(g => g.IsActive) // Active first
                                 .ThenBy(g => g.GoalType)
                                 .ToListAsync(cancellationToken);
        }

        public void Update(Goal goal)
        {
            _context.Goals.Update(goal);
        }

        public void Remove(Goal goal)
        {
            _context.Goals.Remove(goal);
        }
        public IQueryable<Goal> GetQueryableByUserId(int userId)
        {
            return _context.Goals
                           .Where(g => g.UserId == userId)
                           .AsNoTracking(); // Dùng AsNoTracking() vì đây là query chỉ đọc
        }
        
    }
}
