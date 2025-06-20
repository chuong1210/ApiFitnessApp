using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class WorkoutStep : AuditableEntity // Có thể không cần AuditableEntity cho Step
    {
        public int WorkoutStepId { get; private set; } // Khóa chính
        public int WorkoutId { get; private set; }     // Khóa ngoại đến Workout

        public string StepNumber { get; private set; } = string.Empty; // Ví dụ: "01", "02"
        public string Title { get; private set; } = string.Empty;       // Ví dụ: "Spread Your Arms"
        public string Detail { get; private set; } = string.Empty;      // Mô tả chi tiết của bước

        // Navigation property ngược lại Workout (quan trọng cho EF Core)
        public virtual Workout Workout { get; private set; } = null!;

        // Private constructor cho EF Core
        private WorkoutStep() { }

        // Factory method để tạo (tùy chọn)
        public static WorkoutStep Create(int workoutId, string stepNumber, string title, string detail)
        {
            // Thêm validation nếu cần
            return new WorkoutStep
            {
                WorkoutId = workoutId,
                StepNumber = stepNumber,
                Title = title,
                Detail = detail
            };
        }
    }
    }
