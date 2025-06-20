using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWorkoutPlanItemRepository
    {
        /// </summary>
        Task<WorkoutPlanItem?> GetByIdAsync(int planItemId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all items associated with a specific workout plan, ordered by ItemOrder.
        /// </summary>
        Task<IEnumerable<WorkoutPlanItem>> GetByPlanIdAsync(int planId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new workout plan item.
        /// </summary>
        Task AddAsync(WorkoutPlanItem item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple workout plan items.
        /// </summary>
        Task AddRangeAsync(IEnumerable<WorkoutPlanItem> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a workout plan item entity as modified.
        /// </summary>
        void Update(WorkoutPlanItem item);

        /// <summary>
        /// Marks a workout plan item entity for removal.
        /// </summary>
        void Remove(WorkoutPlanItem item);

        /// <summary>
        /// Marks multiple workout plan item entities for removal.
        /// </summary>
        void RemoveRange(IEnumerable<WorkoutPlanItem> items);


        IQueryable<WorkoutPlanItem> GetAllQueryable();

    }

}