using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CompletedWorkoutItem // Không cần AuditableEntity vì là con của WorkoutSession
    {
        public int CompletedWorkoutItemId { get; private set; }
        public int WorkoutSessionId { get; private set; } // FK đến WorkoutSession
        public int WorkoutId { get; private set; } // FK đến Workout (để biết tên, mô tả bài tập)

        public int? SetsDone { get; private set; }
        public int? RepsDone { get; private set; } // Có thể là list reps cho mỗi set: "12,10,8"
        public int? DurationAchievedSeconds { get; private set; }
        public string? Notes { get; private set; } // Ghi chú riêng cho bài tập này trong session

        public virtual WorkoutSession WorkoutSession { get; private set; } = null!;
        public virtual Workout Workout { get; private set; } = null!;

        private CompletedWorkoutItem() { }

        public static CompletedWorkoutItem Create(int workoutSessionId, int workoutId, int? setsDone, int? repsDone, int? durationAchievedSeconds, string? notes)
        {
            return new CompletedWorkoutItem
            {
                WorkoutSessionId = workoutSessionId,
                WorkoutId = workoutId,
                SetsDone = setsDone,
                RepsDone = repsDone,
                DurationAchievedSeconds = durationAchievedSeconds,
                Notes = notes
            };
        }
    }
    }
