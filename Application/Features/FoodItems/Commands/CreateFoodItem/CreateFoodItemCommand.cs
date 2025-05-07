using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.ValidationAttributes; // Thêm using
using System.ComponentModel.DataAnnotations;
using AllowedValuesAttribute = Application.Common.ValidationAttributes.AllowedValuesAttribute; // Cần cho Required

namespace Application.Features.FoodItems.Commands.CreateFoodItem
{

    public record CreateFoodItemCommand(
        string Name,

    [AllowedValues(true, "Fruit", "Vegetable", "Meat", "Dairy", "Grain", "Snack", "Beverage", "Recipe", "Other")] // Giá trị cho phép, không phân biệt hoa thường
        string? Category,
        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10,000.")] // Dùng Range chuẩn
        double CaloriesPerServing,
        string ServingSizeDescription,
        double? ProteinGrams,
        double? CarbGrams,
        double? FatGrams,
        IFormFile? ImageFile // Thuộc tính để nhận file ảnh upload
    ) : IRequest<IResult<FoodItemDto>>; // Trả về DTO của item đã tạo
}
