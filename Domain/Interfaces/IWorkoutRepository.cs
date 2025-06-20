using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IWorkoutRepository
    {
        /// <summary>
        /// Gets a workout by its unique identifier.
        /// </summary>
        Task<Workout?> GetByIdAsync(int workoutId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a workout by its name.
        /// </summary>
        Task<Workout?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all workouts.
        /// </summary>
        Task<IEnumerable<Workout>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Searches workouts by name.
        /// </summary>
        Task<IEnumerable<Workout>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new workout.
        /// </summary>
        Task AddAsync(Workout workout, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing workout.
        /// </summary>
        void Update(Workout workout);

        /// <summary>
        /// Removes a workout.
        /// </summary>
        void Remove(Workout workout);

        // --- CÁC PHƯƠNG THỨC BỔ SUNG ---

        /// <summary>
        /// Gets an IQueryable of all workouts, allowing for further filtering/sorting before execution.
        /// This is useful for building complex queries in the Application layer (e.g., for pagination).
        /// </summary>
        /// <returns>An IQueryable of Workout entities.</returns>
        IQueryable<Workout> GetAllQueryable();

        /// <summary>
        /// Gets a workout by its ID, including its associated steps.
        /// </summary>
        /// <param name="workoutId">The ID of the workout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The workout entity with its steps, or null if not found.</returns>
        Task<Workout?> GetByIdWithStepsAsync(int workoutId, CancellationToken cancellationToken = default);
    }
}
