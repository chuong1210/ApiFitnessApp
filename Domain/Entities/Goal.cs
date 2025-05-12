using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

   
    public class Goal : AuditableEntity
    {
        public int GoalId { get; private set; }
        public int UserId { get; private set; }
        public GoalType GoalType { get; private set; } // Type of goal (e.g., steps, water)
        public double TargetValue { get; private set; } // The target number
        public DateOnly StartDate { get; private set; }
        public DateOnly? EndDate { get; private set; } // Optional end date for the goal
        public bool IsActive { get; private set; } // Is this goal currently being tracked?

        // Navigation property
        public virtual User User { get; private set; } = null!;

        private Goal() { } // For EF Core

        public static Goal Create(int userId, GoalType goalType, double targetValue, DateOnly startDate, DateOnly? endDate = null, bool isActive = true)
        {
            if (targetValue <= 0) throw new ArgumentOutOfRangeException(nameof(targetValue), "Target value must be positive.");
            if (endDate.HasValue && endDate.Value < startDate) throw new ArgumentException("End date cannot be earlier than start date.", nameof(endDate));

            return new Goal
            {
                UserId = userId,
                GoalType = goalType,
                TargetValue = targetValue,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = isActive
            };
        }

        public void UpdateDetails(GoalType goalType, double targetValue, DateOnly startDate, DateOnly? endDate = null)
        {
            if (targetValue <= 0) throw new ArgumentOutOfRangeException(nameof(targetValue), "Target value must be positive.");
            if (endDate.HasValue && endDate.Value < startDate) throw new ArgumentException("End date cannot be earlier than start date.", nameof(endDate));

            GoalType = goalType;
            TargetValue = targetValue;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
