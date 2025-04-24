using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class User
    {
        public int UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Email { get; private set; }
        public string? PasswordHash { get; private set; } // Store hashed password
        public DateOnly? BirthDate { get; private set; }
        public Gender? Gender { get; private set; }
        public double? HeightCm { get; private set; }
        public double? WeightKg { get; private set; } // Can be updated frequently
        public DateTime CreatedAt { get; private set; }
        public bool IsPremium { get; private set; } = false; // Hoặc dùng Enum SubscriptionType
        public bool EmailVerified { get; private set; } = false; // Mặc định là chưa xác thực


        // Navigation properties
        public virtual ICollection<WorkoutSession> WorkoutSessions { get; private set; } = new List<WorkoutSession>();
        public virtual ICollection<DailyActivity> DailyActivities { get; private set; } = new List<DailyActivity>();
        public virtual ICollection<MealLog> MealLogs { get; private set; } = new List<MealLog>();
        public virtual ICollection<ScheduledMeal> ScheduledMeals { get; private set; } = new List<ScheduledMeal>();
        public virtual ICollection<SleepLog> SleepLogs { get; private set; } = new List<SleepLog>();
        public virtual ICollection<Goal> Goals { get; private set; } = new List<Goal>();


        // Private constructor for EF Core
        private User() { }

        // Factory method for creating new users
        public static User Create(string name, string? email, DateOnly? birthDate, Gender? gender, double? heightCm, double? weightKg, string? passwordHash = null)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name is required.", nameof(name));

            return new User
            {
                Name = name,
                Email = email, // Add email format validation if necessary
                BirthDate = birthDate,
                Gender = gender,
                HeightCm = heightCm > 0 ? heightCm : null, // Ensure positive height
                WeightKg = weightKg > 0 ? weightKg : null, // Ensure positive weight
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow ,// Consider using an IDateTimeProvider,
                IsPremium = false, // Giữ nguyên mặc định
                EmailVerified = false // Đặt mặc định là false khi tạo mới
            };
        }

        // Methods for domain logic
        public void UpdateProfile(string name, DateOnly? birthDate, Gender? gender, double? heightCm)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name is required.", nameof(name));
            Name = name;
            BirthDate = birthDate;
            Gender = gender;
            HeightCm = heightCm > 0 ? heightCm : null;
        }
        public void SetPremiumStatus(bool isPremium)
        {
            IsPremium = isPremium;
            // Có thể thêm logic khác ở đây (domain events?)
        }

        public void MarkEmailAsVerified()
        {
            if (!EmailVerified) // Chỉ thay đổi nếu chưa được xác thực
            {
                EmailVerified = true;
                // Có thể thêm Domain Event ở đây nếu cần xử lý logic khác khi email được xác thực
                // AddDomainEvent(new UserEmailVerifiedEvent(this.UserId));
            }
        }
        public void UpdateWeight(double weightKg)
        {
            if (weightKg <= 0) throw new ArgumentException("Weight must be positive.", nameof(weightKg));
            WeightKg = weightKg;
        }

        public void ChangePassword(string newPasswordHash)
        {
            // Add validation for password strength if needed before hashing
            if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new ArgumentException("Password hash cannot be empty.", nameof(newPasswordHash));
            PasswordHash = newPasswordHash;
        }
    }

    // FitnessAp
}
