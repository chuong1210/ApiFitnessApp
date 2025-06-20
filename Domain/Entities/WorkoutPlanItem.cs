using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class WorkoutPlanItem : AuditableEntity
    {
        public int PlanItemId { get; private set; }

        // Foreign Keys
        public int PlanId { get; private set; }
        public int WorkoutId { get; private set; }


        public int ItemOrder { get; private set; } // Order of the workout in the plan
        public int? Sets { get; private set; }
        public int? Reps { get; private set; } // Specific reps for this workout in this plan
        public int? DurationSeconds { get; private set; } // Specific duration for this workout in this plan
        public int? RestBetweenSetsSeconds { get; private set; }

        // Navigation properties back to the related entities
        public virtual WorkoutPlan Plan { get; private set; } = null!; // Required relationship
        public virtual Workout Workout { get; private set; } = null!; // Required relationship
        public int SetNumber { get; private set; } = 1; // Mặc định là set 1 nếu không chỉ định

        private WorkoutPlanItem() { } // For EF Core

        public static WorkoutPlanItem Create(int planId, int setNumber,int workoutId, int itemOrder, int? sets, int? reps, int? durationSeconds, int? restBetweenSetsSeconds)
        {
            if (itemOrder < 0) throw new ArgumentOutOfRangeException(nameof(itemOrder), "Item order must be non-negative.");

            return new WorkoutPlanItem
            {
                PlanId = planId,
                SetNumber = setNumber,
                WorkoutId = workoutId,
                ItemOrder = itemOrder,
                Sets = sets > 0 ? sets : null,
                Reps = reps > 0 ? reps : null,
                DurationSeconds = durationSeconds > 0 ? durationSeconds : null,
                RestBetweenSetsSeconds = restBetweenSetsSeconds >= 0 ? restBetweenSetsSeconds : null // Rest can be 0
            };
        }

        public void UpdateDetails(int itemOrder,int setNumber, int? sets, int? reps, int? durationSeconds, int? restBetweenSetsSeconds)
        {
            if (itemOrder < 0) throw new ArgumentOutOfRangeException(nameof(itemOrder), "Item order must be non-negative.");
            ItemOrder = itemOrder;
            SetNumber = setNumber;
            Sets = sets > 0 ? sets : null;
            Reps = reps > 0 ? reps : null;
            DurationSeconds = durationSeconds > 0 ? durationSeconds : null;
            RestBetweenSetsSeconds = restBetweenSetsSeconds >= 0 ? restBetweenSetsSeconds : null;
        }
    }
}
