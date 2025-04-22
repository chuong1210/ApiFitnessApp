using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Workout
    {
        public int WorkoutId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string? TargetMuscleGroup { get; private set; }
        public int? DefaultReps { get; private set; } // Nullable if not always applicable
        public int? DefaultDurationSeconds { get; private set; } // Nullable if not always applicable
        public string? VideoUrl { get; private set; }
        public string? ImageUrl { get; private set; }

        // Navigation property for relationship with WorkoutPlanItems
        public virtual ICollection<WorkoutPlanItem> WorkoutPlanItems { get; private set; } = new List<WorkoutPlanItem>();

        private Workout() { } // For EF Core

        public static Workout Create(string name, string? description, string? targetMuscleGroup, int? defaultReps, int? defaultDurationSeconds, string? videoUrl = null, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Workout name is required.", nameof(name));

            return new Workout
            {
                Name = name,
                Description = description,
                TargetMuscleGroup = targetMuscleGroup,
                DefaultReps = defaultReps > 0 ? defaultReps : null,
                DefaultDurationSeconds = defaultDurationSeconds > 0 ? defaultDurationSeconds : null,
                VideoUrl = videoUrl,
                ImageUrl = imageUrl
            };
        }
        // Method to update workout details
        public void UpdateDetails(string name, string? description, string? targetMuscleGroup, int? defaultReps, int? defaultDurationSeconds, string? videoUrl = null, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Workout name is required.", nameof(name));
            Name = name;
            Description = description;
            TargetMuscleGroup = targetMuscleGroup;
            DefaultReps = defaultReps > 0 ? defaultReps : null;
            DefaultDurationSeconds = defaultDurationSeconds > 0 ? defaultDurationSeconds : null;
            VideoUrl = videoUrl;
            ImageUrl = imageUrl;
        }
    }

}
