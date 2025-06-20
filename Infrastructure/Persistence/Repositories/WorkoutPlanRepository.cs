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

    public class WorkoutPlanRepository : IWorkoutPlanRepository
    {
        private readonly AppDbContext _context;

        public WorkoutPlanRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<WorkoutPlan?> GetByIdAsync(int planId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPlans.FindAsync(new object[] { planId }, cancellationToken);
        }

        public async Task<WorkoutPlan?> GetByIdWithItemsAsync(int planId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPlans
                                 .Include(wp => wp.Items)                 // Include the PlanItems collection
                                    .ThenInclude(item => item.Workout)  // Then include the Workout for each item
                                 .AsSplitQuery() // Optional: Can improve performance for complex includes
                                 .FirstOrDefaultAsync(wp => wp.PlanId == planId, cancellationToken);
        }

        public async Task<IEnumerable<WorkoutPlan>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPlans.OrderBy(wp => wp.Name).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<WorkoutPlan>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync(cancellationToken);
            }
            var lowerSearchTerm = searchTerm.Trim().ToLower();
            return await _context.WorkoutPlans
                                 .Where(wp => wp.Name.ToLower().Contains(lowerSearchTerm))
                                 .OrderBy(wp => wp.Name)
                                 .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default)
        {
            await _context.WorkoutPlans.AddAsync(workoutPlan, cancellationToken);
            // Note: If workoutPlan.Items are added before calling AddAsync, EF Core might track them.
        }

        public void Update(WorkoutPlan workoutPlan)
        {
            // Be careful when updating plans with items. You might need specific logic
            // in your service layer to handle adding/removing/updating items correctly
            // before calling Update on the plan itself.
            _context.WorkoutPlans.Update(workoutPlan);
        }

        public void Remove(WorkoutPlan workoutPlan)
        {
            // Cascade delete configuration should handle removing associated WorkoutPlanItems
            _context.WorkoutPlans.Remove(workoutPlan);
        }

        public IQueryable<WorkoutPlan> GetAllQueryable()
        {
            // Sử dụng AsNoTracking() vì phương thức này được thiết kế cho các truy vấn
            // chỉ đọc (read-only) và không cần EF Core theo dõi các thay đổi.
            return _context.WorkoutPlans.AsNoTracking();
        }

        public async Task<WorkoutPlan?> GetByIdWithDetailsAsync(int planId, CancellationToken cancellationToken = default)
        {
            return await _context.WorkoutPlans
                // Include danh sách các items (liên kết) trong plan
                .Include(wp => wp.Items)
                    // Với mỗi item, include thông tin chi tiết của bài tập (Workout) liên quan
                    .ThenInclude(item => item.Workout)
                .AsNoTracking() // Dùng AsNoTracking vì chỉ đọc
                .FirstOrDefaultAsync(wp => wp.PlanId == planId, cancellationToken);
        }
    }
    }
