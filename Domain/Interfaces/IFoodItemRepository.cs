using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFoodItemRepository
    {
        /// <summary>
        /// Gets a food item by its unique identifier.
        /// </summary>
        Task<FoodItem?> GetByIdAsync(int foodId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a food item by its exact name (case-insensitive).
        /// </summary>
        Task<FoodItem?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all food items from the library.
        /// </summary>
        Task<IEnumerable<FoodItem>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all food items from the library.
        /// </summary>
        IQueryable<FoodItem> GetAllQueryable();


        /// <summary>
        /// Searches for food items where the name contains the search term (case-insensitive).
        /// </summary>
        Task<IEnumerable<FoodItem>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets food items belonging to a specific category.
        /// </summary>
        Task<IEnumerable<FoodItem>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new food item to the library.
        /// </summary>
        Task AddAsync(FoodItem foodItem, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a food item entity as modified.
        /// </summary>
        void Update(FoodItem foodItem);

        /// <summary>
        /// Marks a food item entity for removal.
        /// </summary>
        void Remove(FoodItem foodItem);
        Task<bool> IsNameUniqueAsync(string name, int currentIdToExclude = 0, CancellationToken cancellationToken = default);
    }
}
