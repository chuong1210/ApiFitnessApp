using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkoutPlan : AuditableEntity
    {
        public int PlanId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public int? EstimatedDurationMinutes { get; private set; }
        public int? EstimatedCaloriesBurned { get; private set; }

        // Navigation property for the items (workouts) within this plan
        public virtual ICollection<WorkoutPlanItem> Items { get; private set; } = new List<WorkoutPlanItem>();

        private WorkoutPlan() { } // For EF Core

        public static WorkoutPlan Create(string name, string? description, int? estimatedDurationMinutes, int? estimatedCaloriesBurned)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Plan name is required.", nameof(name));
            return new WorkoutPlan
            {
                Name = name,
                Description = description,
                EstimatedDurationMinutes = estimatedDurationMinutes > 0 ? estimatedDurationMinutes : null,
                EstimatedCaloriesBurned = estimatedCaloriesBurned > 0 ? estimatedCaloriesBurned : null
            };
        }

        public void UpdateDetails(string name, string? description, int? estimatedDurationMinutes, int? estimatedCaloriesBurned)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Plan name is required.", nameof(name));
            Name = name;
            Description = description;
            EstimatedDurationMinutes = estimatedDurationMinutes > 0 ? estimatedDurationMinutes : null;
            EstimatedCaloriesBurned = estimatedCaloriesBurned > 0 ? estimatedCaloriesBurned : null;
            // Note: Updating Items collection needs separate logic (Add/Remove Item methods)
        }
        // Methods to manage Items (e.g., AddWorkoutItem, RemoveWorkoutItem) would go here
    }


}
