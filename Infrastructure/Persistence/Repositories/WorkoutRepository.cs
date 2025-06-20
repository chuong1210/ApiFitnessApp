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
    public class WorkoutRepository : IWorkoutRepository
    {
        private readonly AppDbContext _context;

        public WorkoutRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Workout?> GetByIdAsync(int workoutId, CancellationToken cancellationToken = default)
        {
            return await _context.Workouts.FindAsync(new object[] { workoutId }, cancellationToken);
        }

        public async Task<Workout?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Workouts
                                 .FirstOrDefaultAsync(w => w.Name.ToLower() == name.ToLower(), cancellationToken);
        }

        public async Task<IEnumerable<Workout>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Workouts.OrderBy(w => w.Name).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Workout>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync(cancellationToken);
            }
            var lowerSearchTerm = searchTerm.Trim().ToLower();
            return await _context.Workouts
                                 .Where(w => w.Name.ToLower().Contains(lowerSearchTerm))
                                 .OrderBy(w => w.Name)
                                 .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Workout workout, CancellationToken cancellationToken = default)
        {
            await _context.Workouts.AddAsync(workout, cancellationToken);
        }

        public void Update(Workout workout)
        {
            _context.Workouts.Update(workout);
        }

        public void Remove(Workout workout)
        {
            _context.Workouts.Remove(workout);
        }

        /// <summary>
        /// Gets an IQueryable of all workouts for read-only purposes.
        /// </summary>
        public IQueryable<Workout> GetAllQueryable()
        {
            // Sử dụng AsNoTracking() vì phương thức này được thiết kế cho các truy vấn
            // chỉ đọc (read-only) và không cần EF Core theo dõi các thay đổi.
            return _context.Workouts.AsNoTracking();
        }

        /// <summary>
        /// Gets a workout by its ID, including its associated steps.
        /// </summary>
        public async Task<Workout?> GetByIdWithStepsAsync(int workoutId, CancellationToken cancellationToken = default)
        {
            // Giả định rằng Workout entity của bạn có một navigation property tên là "Steps"
            // trỏ đến một collection các WorkoutStep entities.
            // Ví dụ: public virtual ICollection<WorkoutStep> Steps { get; set; } = new List<WorkoutStep>();
            return await _context.Workouts
                                 .Include(w => w.Steps) // Eagerly load the related Steps
                                 .FirstOrDefaultAsync(w => w.WorkoutId == workoutId, cancellationToken);
        }
    }
}
