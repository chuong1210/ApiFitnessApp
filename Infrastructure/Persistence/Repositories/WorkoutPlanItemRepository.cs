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
    public class WorkoutPlanItemRepository : IWorkoutPlanItemRepository
    {
        private readonly AppDbContext _context;

        public WorkoutPlanItemRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<WorkoutPlanItem?> GetByIdAsync(int planItemId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPlanItems.FindAsync(new object[] { planItemId }, cancellationToken);
        }

        public async Task<IEnumerable<WorkoutPlanItem>> GetByPlanIdAsync(int planId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPlanItems
                                 .Where(wpi => wpi.PlanId == planId)
                                 .OrderBy(wpi => wpi.ItemOrder) // Ensure correct order
                                 .Include(wpi => wpi.Workout) // Include workout details if needed often
                                 .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(WorkoutPlanItem item, CancellationToken cancellationToken = default)
        {
            await _context.WorkoutPlanItems.AddAsync(item, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<WorkoutPlanItem> items, CancellationToken cancellationToken = default)
        {
            await _context.WorkoutPlanItems.AddRangeAsync(items, cancellationToken);
        }

        public void Update(WorkoutPlanItem item)
        {
            _context.WorkoutPlanItems.Update(item);
        }

        public void Remove(WorkoutPlanItem item)
        {
            _context.WorkoutPlanItems.Remove(item);
        }

        public void RemoveRange(IEnumerable<WorkoutPlanItem> items)
        {
            _context.WorkoutPlanItems.RemoveRange(items);
        }
    }
    }
