using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ScheduledMeal
    {
        public int ScheduleId { get; private set; }
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; } // The day the meal is scheduled for
        public MealType MealType { get; private set; } // Scheduled for Breakfast, Lunch, etc.

        // Can link to a specific FoodItem OR just have a description
        public int? PlannedFoodId { get; private set; }
        public string? PlannedDescription { get; private set; } // If not linking to a specific FoodItem

        public ScheduleStatus Status { get; private set; } // Planned, Eaten, Skipped

        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual FoodItem? PlannedFoodItem { get; private set; } // Nullable FK

        private ScheduledMeal() { } // For EF Core

        public static ScheduledMeal Create(int userId, DateOnly date, MealType mealType, int? plannedFoodId, string? plannedDescription, ScheduleStatus status = ScheduleStatus.Planned)
        {
            if (plannedFoodId == null && string.IsNullOrWhiteSpace(plannedDescription))
                throw new ArgumentException("Either a planned Food ID or a description is required for a scheduled meal.");

            return new ScheduledMeal
            {
                UserId = userId,
                Date = date,
                MealType = mealType,
                PlannedFoodId = plannedFoodId,
                PlannedDescription = plannedDescription,
                Status = status
            };
        }

        public void UpdateDetails(DateOnly date, MealType mealType, int? plannedFoodId, string? plannedDescription)
        {
            if (plannedFoodId == null && string.IsNullOrWhiteSpace(plannedDescription))
                throw new ArgumentException("Either a planned Food ID or a description is required for a scheduled meal.");

            Date = date;
            MealType = mealType;
            PlannedFoodId = plannedFoodId;
            PlannedDescription = plannedDescription;
            // Status might be updated separately
        }

        public void UpdateStatus(ScheduleStatus newStatus)
        {
            Status = newStatus;
        }
    }


}
