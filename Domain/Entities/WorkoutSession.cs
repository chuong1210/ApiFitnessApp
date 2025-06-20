using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
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
        public int? CustomReps { get; private set; } // Số lần lặp tùy chỉnh
        public double? CustomWeight { get; private set; } // Mức tạ tùy chỉnh (dùng double hoặc decimal)

        public SessionStatus Status { get; private set; }

        private WorkoutSession() { } // For EF Core
        public static WorkoutSession CreateScheduled(
            int userId,
            DateTime scheduledTime,
            int workoutId,
            int? customReps,
            double? customWeight)
        {
            // Kiểm tra các tham số đầu vào
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (workoutId <= 0) throw new ArgumentOutOfRangeException(nameof(workoutId));
            // Kiểm tra scheduledTime phải trong tương lai (tùy chọn)
            if (scheduledTime < DateTime.UtcNow)
            {
                // Có thể ném lỗi hoặc chỉ ghi log cảnh báo tùy yêu cầu
                // throw new ArgumentException("Scheduled time must be in the future.", nameof(scheduledTime));
            }

            var session = new WorkoutSession
            {
                UserId = userId,
                WorkoutId = workoutId,
                StartTime = scheduledTime, // Gán thời gian lên lịch vào StartTime
                Status = SessionStatus.Scheduled, // Đặt trạng thái là "Đã lên lịch"

                // Gán các giá trị tùy chỉnh
                CustomReps = customReps,
                CustomWeight = customWeight,

                // Các trường khác sẽ là null hoặc giá trị mặc định lúc này
                PlanId = null,
                EndTime = DateTime.UtcNow,
                DurationSeconds = 0,
                CaloriesBurned = null,
                AvgHeartRate = null,
                MaxHeartRate = null,
                Notes = null
                // Các trường AuditableEntity sẽ được Interceptor xử lý
            };

            return session;
        }

        // (Tùy chọn) Thêm các phương thức khác để quản lý trạng thái
        public void MarkAsCompleted(DateTime endTime, int? calories = null, string? notes = null)
        {
            if (Status == SessionStatus.Scheduled || Status == SessionStatus.InProgress)
            {
                Status = SessionStatus.Completed;
                EndTime = endTime;
                DurationSeconds = (int)(endTime - this.StartTime).TotalSeconds;
                CaloriesBurned = calories;
                Notes = notes;
            }
            // Có thể ném lỗi nếu cố gắng hoàn thành một session đã hoàn thành
        }
        // --- PHƯƠNG THỨC CreateCompleted ĐẦY ĐỦ ---
        /// <summary>
        /// Creates a new, completed workout session log.
        /// </summary>
        /// <param name="userId">The ID of the user who completed the workout.</param>
        /// <param name="planId">The ID of the plan, if the workout was part of a plan.</param>
        /// <param name="workoutId">The ID of the single workout, if it was not part of a plan.</param>
        /// <param name="startTime">The time the workout started.</param>
        /// <param name="endTime">The time the workout finished.</param>
        /// <param name="durationSeconds">The total duration in seconds.</param>
        /// <param name="caloriesBurned">Estimated calories burned.</param>
        /// <param name="notes">User's notes about the session.</param>
        /// <returns>A new WorkoutSession instance with 'Completed' status.</returns>
        public static WorkoutSession CreateCompleted(
            int userId,
            int? planId,
            int? workoutId,
            DateTime startTime,
            DateTime endTime,
            int durationSeconds,
            int? caloriesBurned,
            string? notes)
        {
            // --- Validation đầu vào ---
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId), "User ID must be positive.");
            if (planId == null && workoutId == null)
                throw new ArgumentException("Either a Plan ID or a Workout ID must be provided.");
            if (endTime < startTime)
                throw new ArgumentException("End time cannot be earlier than start time.", nameof(endTime));
            if (durationSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(durationSeconds), "Duration cannot be negative.");
            if (caloriesBurned.HasValue && caloriesBurned < 0)
                throw new ArgumentOutOfRangeException(nameof(caloriesBurned), "Calories burned cannot be negative.");

            // Tạo một instance mới của WorkoutSession
            var session = new WorkoutSession
            {
                UserId = userId,
                PlanId = planId,
                WorkoutId = workoutId,
                StartTime = startTime,
                EndTime = endTime,
                DurationSeconds = durationSeconds,
                CaloriesBurned = caloriesBurned,
                Notes = notes,
                Status = SessionStatus.Completed, // Đặt trạng thái là "Đã hoàn thành"

                // Các trường không áp dụng cho một buổi tập đã log có thể để null
                CustomReps = null,
                CustomWeight = null,
                AvgHeartRate = null, // Giả sử không có dữ liệu này từ client
                MaxHeartRate = null
                // Các trường AuditableEntity sẽ được Interceptor xử lý
            };

            return session;
        }
        public void MarkAsSkipped()
        {
            if (Status == SessionStatus.Scheduled)
            {
                Status = SessionStatus.Skipped;
            }
        }
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
