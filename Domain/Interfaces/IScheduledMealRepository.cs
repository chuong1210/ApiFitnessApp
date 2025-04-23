using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IScheduledMealRepository
    {
        /// <summary>
        /// Gets a scheduled meal entry by its unique identifier.
        /// </summary>
        Task<ScheduledMeal?> GetByIdAsync(int scheduleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new scheduled meal entry.
        /// </summary>
        Task AddAsync(ScheduledMeal scheduledMeal, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all scheduled meals for a specific user on a specific date.
        /// </summary>
        Task<IEnumerable<ScheduledMeal>> GetByUserIdAndDateAsync(int userId, DateOnly date, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets scheduled meals for a specific user within a specified date range.
        /// </summary>
        Task<IEnumerable<ScheduledMeal>> GetByUserIdAndDateRangeAsync(int userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a scheduled meal entity as modified.
        /// </summary>
        void Update(ScheduledMeal scheduledMeal);

        /// <summary>
        /// Marks a scheduled meal entity for removal.
        /// </summary>
        void Remove(ScheduledMeal scheduledMeal);
    }


 
    }

