using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    // FitnessApp.Domain/Entities/BodyMeasurement.cs
  

    public class BodyMeasurement : AuditableEntity
    {
        public int MeasurementId { get; private set; }
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public double? WeightKg { get; private set; }
        public double? BodyFatPercentage { get; private set; }
        public double? MuscleMassKg { get; private set; }
        public double? WaistCm { get; private set; }
        public double? ChestCm { get; private set; }
        public double? HipsCm { get; private set; }
        // Thêm các số đo khác nếu cần (ví dụ: ArmLeftCm, ArmRightCm, ThighLeftCm, ThighRightCm)
        public string? Notes { get; private set; }

        public virtual User User { get; private set; } = null!;

        private BodyMeasurement() { }

        public static BodyMeasurement Create(
            int userId, DateOnly date, double? weightKg, double? bodyFatPercentage,
            double? muscleMassKg, double? waistCm, double? chestCm, double? hipsCm,
            string? notes)
        {
            // Thêm validation cơ bản nếu cần (ví dụ: giá trị không âm)
            if (date > DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                throw new ArgumentOutOfRangeException(nameof(date), "Measurement date cannot be too far in the future.");

            return new BodyMeasurement
            {
                UserId = userId,
                Date = date,
                WeightKg = weightKg,
                BodyFatPercentage = bodyFatPercentage,
                MuscleMassKg = muscleMassKg,
                WaistCm = waistCm,
                ChestCm = chestCm,
                HipsCm = hipsCm,
                Notes = notes
            };
        }

        public void UpdateDetails(
            DateOnly date, double? weightKg, double? bodyFatPercentage,
            double? muscleMassKg, double? waistCm, double? chestCm, double? hipsCm,
            string? notes)
        {
            if (date > DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                throw new ArgumentOutOfRangeException(nameof(date), "Measurement date cannot be too far in the future.");

            Date = date;
            WeightKg = weightKg;
            BodyFatPercentage = bodyFatPercentage;
            MuscleMassKg = muscleMassKg;
            WaistCm = waistCm;
            ChestCm = chestCm;
            HipsCm = hipsCm;
            Notes = notes;
        }
    }
}
