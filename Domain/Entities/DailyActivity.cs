using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DailyActivity
    {
        public int ActivityId { get; private set; }
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; } // The specific day for this record
        public int StepsCount { get; private set; }
        public int WaterIntakeMl { get; private set; }
        public int SleepDurationMinutes { get; private set; }
        public int ActiveCaloriesBurned { get; private set; } // Calories from workouts/activity
        public int RestingCaloriesBurned { get; private set; } // BMR, potentially calculated elsewhere

        // Navigation property
        public virtual User User { get; private set; } = null!;

        private DailyActivity() { } // For EF Core

        // Factory to create a record for a day (usually starts with 0 values)
        public static DailyActivity Create(int userId, DateOnly date)
        {
            return new DailyActivity
            {
                UserId = userId,
                Date = date,
                StepsCount = 0,
                WaterIntakeMl = 0,
                SleepDurationMinutes = 0,
                ActiveCaloriesBurned = 0,
                RestingCaloriesBurned = 0 // Needs to be set externally if calculated
            };
        }

        // Methods to update daily values
        public void UpdateSteps(int steps) => StepsCount = steps >= 0 ? steps : 0;
        public void AddWater(int ml) => WaterIntakeMl = Math.Max(0, WaterIntakeMl + ml); // Additive
        public void SetWater(int ml) => WaterIntakeMl = ml >= 0 ? ml : 0; // Set total
        public void UpdateSleep(int minutes) => SleepDurationMinutes = minutes >= 0 ? minutes : 0;
        public void AddActiveCalories(int calories) => ActiveCaloriesBurned = Math.Max(0, ActiveCaloriesBurned + calories); // Additive
        public void SetActiveCalories(int calories) => ActiveCaloriesBurned = calories >= 0 ? calories : 0; // Set total
        public void SetRestingCalories(int calories) => RestingCaloriesBurned = calories >= 0 ? calories : 0;

    }

}
