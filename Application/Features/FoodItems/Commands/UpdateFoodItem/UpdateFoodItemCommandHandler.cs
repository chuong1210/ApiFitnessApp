using Application.Common.Interfaces;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Commands.UpdateFoodItem
{
    public class UpdateFoodItemCommandHandler : IRequestHandler<UpdateFoodItemCommand, IResult<FoodItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<UpdateFoodItemCommandHandler> _logger;

        public UpdateFoodItemCommandHandler(
            IUnitOfWork unitOfWork, IMapper mapper,
            ICloudinaryService cloudinaryService, ILogger<UpdateFoodItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<IResult<FoodItemDto>> Handle(UpdateFoodItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm FoodItem cần cập nhật
            var foodItem = await _unitOfWork.FoodItems.GetByIdAsync(request.FoodId, cancellationToken);
            if (foodItem == null)
            {
                return Result<FoodItemDto>.Failure($"Food item with ID {request.FoodId} not found.", StatusCodes.Status404NotFound);
            }

            string? oldImagePublicId = foodItem.ImagePublicId; // Lưu lại Public ID cũ (cần thêm trường này vào FoodItem)
            string? newImageUrl = foodItem.ImageUrl; // Giữ URL cũ làm mặc định
            string? newImagePublicId = foodItem.ImagePublicId; // Giữ Public ID cũ làm mặc định

            bool imageChanged = false;

            // 2. Xử lý ảnh
            // 2a. Nếu yêu cầu xóa ảnh hiện tại
            if (request.RemoveCurrentImage && !string.IsNullOrEmpty(oldImagePublicId))
            {
                _logger.LogInformation("Removing current image for FoodItem {FoodItemId}, PublicID: {PublicId}", request.FoodId, oldImagePublicId);
                await _cloudinaryService.DeleteImageAsync(oldImagePublicId);
                newImageUrl = null;
                newImagePublicId = null;
                imageChanged = true;
            }
            // 2b. Nếu có ảnh mới được upload
            else if (request.NewImageFile != null)
            {
                _logger.LogInformation("Uploading new image for FoodItem {FoodItemId}: {FileName}", request.FoodId, request.NewImageFile.FileName);
                var uploadResult = await _cloudinaryService.UploadImageAsync(request.NewImageFile, "food_items");
                if (!uploadResult.IsSuccess)
                {
                    _logger.LogWarning("New image upload failed for FoodItem {FoodItemId}: {ErrorMessage}", request.FoodId, uploadResult.ErrorMessage);
                    return Result<FoodItemDto>.Failure($"New image upload failed: {uploadResult.ErrorMessage}", StatusCodes.Status400BadRequest);
                }

                // Xóa ảnh cũ (nếu có) SAU KHI upload ảnh mới thành công
                if (!string.IsNullOrEmpty(oldImagePublicId))
                {
                    _logger.LogInformation("Deleting old image for FoodItem {FoodItemId}, PublicID: {PublicId}", request.FoodId, oldImagePublicId);
                    await _cloudinaryService.DeleteImageAsync(oldImagePublicId);
                }

                newImageUrl = uploadResult.Url;
                newImagePublicId = uploadResult.PublicId;
                imageChanged = true;
                _logger.LogInformation("New image uploaded for FoodItem {FoodItemId}. URL: {ImageUrl}, PublicID: {PublicId}", request.FoodId, newImageUrl, newImagePublicId);
            }

            // 3. Cập nhật thuộc tính của FoodItem Entity
            // Tạo phương thức UpdateDetails trong FoodItem để đóng gói logic này
            foodItem.UpdateDetails(
                request.Name,
                request.CaloriesPerServing,
                request.ServingSizeDescription,
                request.Category,
                request.ProteinGrams,
                request.CarbGrams,
                request.FatGrams,
                newImageUrl // Chỉ cập nhật URL nếu ảnh đã thay đổi
            );
            // Cập nhật Public ID nếu ảnh thay đổi
            if (imageChanged)
            {
                foodItem.SetImagePublicId(newImagePublicId); // Giả sử có phương thức này
            }


            try
            {
                // 4. Đánh dấu thay đổi và Lưu vào DB
                _unitOfWork.FoodItems.Update(foodItem); // EF Core sẽ tự phát hiện thay đổi
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Food item {FoodItemId} updated successfully.", request.FoodId);

                // 5. Map sang DTO và trả về
                var updatedDto = _mapper.Map<FoodItemDto>(foodItem);
                return Result<FoodItemDto>.Success(updatedDto, StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating food item {FoodItemId}.", request.FoodId);
                // Cân nhắc rollback việc xóa/upload ảnh nếu DB update lỗi (phức tạp hơn)
                return Result<FoodItemDto>.Failure($"An error occurred while updating the food item: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
