using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ScheduledMeals.Dtos
{
    public record ScheduledMealRequestBaseDto(
        [Required]
    DateOnly Date,

        [Required]
    MealType MealType,

        // Chỉ một trong hai trường này nên có giá trị
        int? PlannedFoodId,
        [MaxLength(500)]
    string? PlannedDescription
    );
}
