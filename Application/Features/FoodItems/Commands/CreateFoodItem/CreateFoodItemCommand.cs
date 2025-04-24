using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Commands.CreateFoodItem
{

    public record CreateFoodItemCommand(
        string Name,
        string? Category,
        double CaloriesPerServing,
        string ServingSizeDescription,
        double? ProteinGrams,
        double? CarbGrams,
        double? FatGrams,
        IFormFile? ImageFile // Thuộc tính để nhận file ảnh upload
    ) : IRequest<IResult<FoodItemDto>>; // Trả về DTO của item đã tạo
}
