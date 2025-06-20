using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWorkoutSessionRepository
    {
        /// <summary>
        /// Gets a workout session by its unique identifier.
        /// </summary>
        Task<WorkoutSession?> GetByIdAsync(int sessionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new workout session log.
        /// </summary>
        Task AddAsync(WorkoutSession session, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all workout sessions for a specific user, ordered by start time descending.
        /// </summary>
        Task<IEnumerable<WorkoutSession>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets workout sessions for a specific user within a specified date/time range (based on StartTime).
        /// </summary>
        Task<IEnumerable<WorkoutSession>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a workout session entity as modified.
        /// </summary>
        void Update(WorkoutSession session);

        /// <summary>
        /// Marks a workout session entity for removal.
        /// </summary>
        void Remove(WorkoutSession session);

        /// <summary>
        /// Gets an IQueryable of all workout sessions, allowing for further filtering before execution.
        /// </summary>
        /// <returns>An IQueryable of WorkoutSession entities.</returns>
        IQueryable<WorkoutSession> GetQueryable();
    }
    
}
