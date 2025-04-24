using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FoodItem
    {
        public int FoodId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Category { get; private set; } // e.g., "Fruit", "Vegetable", "Meat", "Recipe", "Snack"
        public double CaloriesPerServing { get; private set; }
        public string ServingSizeDescription { get; private set; } = string.Empty; // e.g., "100g", "1 cup", "1 slice"
        public double? ProteinGrams { get; private set; }
        public double? CarbGrams { get; private set; }
        public double? FatGrams { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? ImagePublicId { get; private set; }


        // Navigation property for logs using this item
        public virtual ICollection<MealLog> MealLogs { get; private set; } = new List<MealLog>();
        public virtual ICollection<ScheduledMeal> ScheduledMeals { get; private set; } = new List<ScheduledMeal>();


        private FoodItem() { } // For EF Core

        public static FoodItem Create(string name, double caloriesPerServing, string servingSizeDescription, string? category = null, double? proteinGrams = null, double? carbGrams = null, double? fatGrams = null, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Food item name is required.", nameof(name));
            if (caloriesPerServing < 0) throw new ArgumentOutOfRangeException(nameof(caloriesPerServing), "Calories cannot be negative.");
            if (string.IsNullOrWhiteSpace(servingSizeDescription)) throw new ArgumentException("Serving size description is required.", nameof(servingSizeDescription));

            return new FoodItem
            {
                Name = name,
                Category = category,
                CaloriesPerServing = caloriesPerServing,
                ServingSizeDescription = servingSizeDescription,
                ProteinGrams = proteinGrams >= 0 ? proteinGrams : null,
                CarbGrams = carbGrams >= 0 ? carbGrams : null,
                FatGrams = fatGrams >= 0 ? fatGrams : null,
                ImageUrl = imageUrl,
                ImagePublicId = null // Khởi tạo là null

            };
        }

        public void UpdateDetails(string name, double caloriesPerServing, string servingSizeDescription, string? category = null, double? proteinGrams = null, double? carbGrams = null, double? fatGrams = null, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Food item name is required.", nameof(name));
            if (caloriesPerServing < 0) throw new ArgumentOutOfRangeException(nameof(caloriesPerServing), "Calories cannot be negative.");
            if (string.IsNullOrWhiteSpace(servingSizeDescription)) throw new ArgumentException("Serving size description is required.", nameof(servingSizeDescription));

            Name = name;
            Category = category;
            CaloriesPerServing = caloriesPerServing;
            ServingSizeDescription = servingSizeDescription;
            ProteinGrams = proteinGrams >= 0 ? proteinGrams : null;
            CarbGrams = carbGrams >= 0 ? carbGrams : null;
            FatGrams = fatGrams >= 0 ? fatGrams : null;
            ImageUrl = imageUrl;
        }
        public void SetImagePublicId(string? publicId)
        {
            ImagePublicId = publicId;
        }
    }

}
