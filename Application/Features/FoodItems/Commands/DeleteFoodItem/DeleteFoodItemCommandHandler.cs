using Application.Common.Interfaces;
using Application.Responses.Interfaces;
using Application.Responses;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Commands.DeleteFoodItem
{

    public class DeleteFoodItemCommandHandler : IRequestHandler<DeleteFoodItemCommand, IResult<int>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<DeleteFoodItemCommandHandler> _logger;

        public DeleteFoodItemCommandHandler(
            IUnitOfWork unitOfWork,
            ICloudinaryService cloudinaryService,
            ILogger<DeleteFoodItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<IResult<int>> Handle(DeleteFoodItemCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm FoodItem cần xóa
            var foodItem = await _unitOfWork.FoodItems.GetByIdAsync(request.FoodId, cancellationToken);
            if (foodItem == null)
            {
                return Result<int>.Failure($"Food item with ID {request.FoodId} not found.", StatusCodes.Status404NotFound);
            }

            // Lấy Public ID của ảnh trước khi xóa entity
            string? imagePublicId = foodItem.ImagePublicId; // Cần có trường này

            try
            {
                // 2. Xóa entity khỏi DB
                // Lưu ý: Cấu hình khóa ngoại trong MealLog (Restrict) sẽ ngăn việc xóa nếu item đang được dùng.
                // EF Core sẽ ném DbUpdateException nếu có ràng buộc.
                _unitOfWork.FoodItems.Remove(foodItem);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Food item {FoodItemId} deleted successfully from database.", request.FoodId);

                // 3. Xóa ảnh khỏi Cloudinary (SAU KHI xóa DB thành công)
                if (!string.IsNullOrEmpty(imagePublicId))
                {
                    _logger.LogInformation("Attempting to delete image from Cloudinary for deleted FoodItem {FoodItemId}, PublicID: {PublicId}", request.FoodId, imagePublicId);
                    bool deletionSuccess = await _cloudinaryService.DeleteImageAsync(imagePublicId);
                    if (!deletionSuccess)
                    {
                        // Ghi log cảnh báo nhưng không nên coi việc xóa item là thất bại
                        _logger.LogWarning("Failed to delete image {PublicId} from Cloudinary after deleting FoodItem {FoodItemId}.", imagePublicId, request.FoodId);
                    }
                }

                // 4. Trả về ID đã xóa
                return Result<int>.Success(request.FoodId, StatusCodes.Status200OK);
            }
            catch (DbUpdateException dbEx) // Bắt lỗi do ràng buộc khóa ngoại (nếu có)
            {
                _logger.LogError(dbEx, "Error deleting food item {FoodItemId}. It might be in use.", request.FoodId);
                // Kiểm tra InnerException để biết chi tiết lỗi ràng buộc
                return Result<int>.Failure($"Could not delete food item with ID {request.FoodId}. It might be used in existing meal logs.", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting food item {FoodItemId}.", request.FoodId);
                return Result<int>.Failure($"An error occurred while deleting the food item: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }

}