using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkoutSession : AuditableEntity
    {
        public int SessionId { get; private set; }
        public int UserId { get; private set; }

        // Can be linked to a specific plan OR a single workout (if not part of a plan)
        public int? PlanId { get; private set; }
        public int? WorkoutId { get; private set; } // Relevant if PlanId is null

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public int DurationSeconds { get; private set; } // Calculated: EndTime - StartTime
        public int? CaloriesBurned { get; private set; }
        public int? AvgHeartRate { get; private set; }
        public int? MaxHeartRate { get; private set; }
        public string? Notes { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; } = null!;
        public virtual WorkoutPlan? Plan { get; private set; } // Session might not belong to a plan
        public virtual Workout? Workout { get; private set; } // Session might be a single workout


        private WorkoutSession() { } // For EF Core

        public static WorkoutSession Log(int userId, DateTime startTime, DateTime endTime, int? planId, int? workoutId, int? caloriesBurned, int? avgHeartRate, int? maxHeartRate, string? notes)
        {
            if (endTime < startTime) throw new ArgumentException("End time cannot be earlier than start time.", nameof(endTime));
            if (planId == null && workoutId == null) throw new ArgumentException("Either PlanId or WorkoutId must be provided for a session.");
            if (planId != null && workoutId != null) throw new ArgumentException("Session cannot belong to both a Plan and a single Workout simultaneously."); // Or adjust logic if needed


            var duration = (int)(endTime - startTime).TotalSeconds;
            if (duration < 0) duration = 0; // Should not happen with check above, but safeguard

            return new WorkoutSession
            {
                UserId = userId,
                StartTime = startTime,
                EndTime = endTime,
                DurationSeconds = duration,
                PlanId = planId,
                WorkoutId = workoutId,
                CaloriesBurned = caloriesBurned > 0 ? caloriesBurned : null,
                AvgHeartRate = avgHeartRate > 0 ? avgHeartRate : null,
                MaxHeartRate = maxHeartRate > 0 ? maxHeartRate : null,
                Notes = notes
            };
        }
        // Method to update notes or potentially other metrics if re-calculated
        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }
    }
}
