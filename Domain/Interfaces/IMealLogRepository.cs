using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMealLogRepository
    {
        /// <summary>
        /// Gets a meal log entry by its unique identifier.
        /// </summary>
        Task<MealLog?> GetByIdAsync(int logId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new meal log entry.
        /// </summary>
        Task AddAsync(MealLog mealLog, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets meal log entries for a specific user within a specified date/time range.
        /// </summary>
        Task<IEnumerable<MealLog>> GetByUserIdAndDateRangeAsync(int userId, DateTime startDateTime, DateTime endDateTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets meal log entries for a specific user on a specific date.
        /// </summary>
        Task<IEnumerable<MealLog>> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a meal log entity as modified.
        /// </summary>
        void Update(MealLog mealLog);

        /// <summary>
        /// Marks a meal log entity for removal.
        /// </summary>
        void Remove(MealLog mealLog);


      IQueryable<MealLog> GetQueryableByUserId(int userId);
        IQueryable<MealLog> GetQueryable();

    }
}
