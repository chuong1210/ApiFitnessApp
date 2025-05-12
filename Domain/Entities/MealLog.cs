using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MealLog : AuditableEntity
    {
        public int LogId { get; private set; }
        public int UserId { get; private set; }
        public int FoodId { get; private set; }
        public DateTime Timestamp { get; private set; } // When the meal was logged/eaten
        public MealType MealType { get; private set; } // Breakfast, Lunch, Dinner, Snack
        public double Quantity { get; private set; } // Number of servings eaten
        public double TotalCalories { get; private set; } // Calculated: Quantity * FoodItem.CaloriesPerServing
        public string? Notes { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual FoodItem FoodItem { get; private set; } = null!;

        private MealLog() { } // For EF Core

        // Note: totalCalories should ideally be calculated based on FoodItem data when creating
        public static MealLog Create(int userId, int foodId, DateTime timestamp, MealType mealType, double quantity, double totalCalories, string? notes = null)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
            if (totalCalories < 0) throw new ArgumentOutOfRangeException(nameof(totalCalories), "Total calories cannot be negative."); // Allow 0

            return new MealLog
            {
                UserId = userId,
                FoodId = foodId,
                Timestamp = timestamp,
                MealType = mealType,
                Quantity = quantity,
                TotalCalories = totalCalories,
                Notes = notes
            };
        }

        public void UpdateDetails(DateTime timestamp, MealType mealType, double quantity, double totalCalories, string? notes = null)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
            if (totalCalories < 0) throw new ArgumentOutOfRangeException(nameof(totalCalories), "Total calories cannot be negative.");

            Timestamp = timestamp;
            MealType = mealType;
            Quantity = quantity;
            TotalCalories = totalCalories; // Recalculate if needed based on quantity/food item
            Notes = notes;
        }
    }


}
