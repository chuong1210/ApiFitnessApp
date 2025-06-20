using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDailyActivityRepository
    {
        /// <summary>
        /// Gets the daily activity record for a specific user and date.
        /// </summary>
        /// <returns>The DailyActivity entity or null if no record exists for that day.</returns>
        Task<DailyActivity?> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new daily activity record.
        /// </summary>
        Task AddAsync(DailyActivity activity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a daily activity entity as modified.
        /// </summary>
        void Update(DailyActivity activity);

        /// <summary>
        /// Gets daily activity records for a specific user within a specified date range.
        /// </summary>
        Task<IEnumerable<DailyActivity>> GetByUserIdAndDateRangeAsync(int userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a daily activity entity for removal.
        /// </summary>
        void Remove(DailyActivity activity);


        /// <summary>
        /// Gets an IQueryable of all workouts, allowing for further filtering/sorting before execution.
        /// This is useful for building complex queries in the Application layer (e.g., for pagination).
        /// </summary>
        /// <returns>An IQueryable of Workout entities.</returns>
        IQueryable<DailyActivity> GetAllQueryable();
    }
}
