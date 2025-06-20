using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWorkoutPlanRepository
    {
        /// <summary>
        /// Gets a workout plan by its unique identifier.
        /// </summary>
        Task<WorkoutPlan?> GetByIdAsync(int planId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a workout plan by its ID, including its associated items and their workout details.
        /// </summary>
        Task<WorkoutPlan?> GetByIdWithItemsAsync(int planId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all workout plans.
        /// </summary>
        Task<IEnumerable<WorkoutPlan>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches for workout plans by name containing the search term.
        /// </summary>
        Task<IEnumerable<WorkoutPlan>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new workout plan. Associated items must be handled separately or within the plan entity graph.
        /// </summary>
        Task AddAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a workout plan entity as modified. Updating items requires specific handling.
        /// </summary>
        void Update(WorkoutPlan workoutPlan);

        /// <summary>
        /// Marks a workout plan entity for removal (cascade should delete items).
        /// </summary>
        void Remove(WorkoutPlan workoutPlan);

        Task<WorkoutPlan?> GetByIdWithDetailsAsync(int planId, CancellationToken cancellationToken = default);


        IQueryable<WorkoutPlan> GetAllQueryable();

    }
}
