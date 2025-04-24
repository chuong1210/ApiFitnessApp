using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses.Dtos
{
    public record FoodItemDto(
        int FoodId,
        string Name,
        string? Category,
        double CaloriesPerServing,
        string ServingSizeDescription,
        double? ProteinGrams,
        double? CarbGrams,
        double? FatGrams,
        string? ImageUrl
    );
}
