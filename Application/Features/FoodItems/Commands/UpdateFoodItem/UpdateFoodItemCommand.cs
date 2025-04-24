using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Commands.UpdateFoodItem
{

    public record UpdateFoodItemCommand(
        int FoodId, // ID của item cần cập nhật
        string Name,
        string? Category,
        double CaloriesPerServing,
        string ServingSizeDescription,
        double? ProteinGrams,
        double? CarbGrams,
        double? FatGrams,
        IFormFile? NewImageFile, // File ảnh mới (nếu muốn thay đổi)
        bool RemoveCurrentImage = false // Cờ để xóa ảnh hiện tại mà không upload ảnh mới
    ) : IRequest<IResult<FoodItemDto>>; // Trả về DTO của item đã cập nhật
}
