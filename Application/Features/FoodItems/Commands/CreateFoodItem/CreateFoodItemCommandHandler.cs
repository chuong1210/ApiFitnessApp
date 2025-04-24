using Application.Common.Interfaces;
using Application.Responses.Dtos;
using Application.Responses.Interfaces;
using Application.Responses;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.FoodItems.Commands.CreateFoodItem
{
    public class CreateFoodItemCommandHandler : IRequestHandler<CreateFoodItemCommand, IResult<FoodItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<CreateFoodItemCommandHandler> _logger;

        public CreateFoodItemCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICloudinaryService cloudinaryService,
            ILogger<CreateFoodItemCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<IResult<FoodItemDto>> Handle(CreateFoodItemCommand request, CancellationToken cancellationToken)
        {
            string? imageUrl = null;
            string? imagePublicId = null; // Lưu lại Public ID để có thể xóa sau này

            // 1. Upload ảnh lên Cloudinary nếu có
            if (request.ImageFile != null)
            {
                _logger.LogInformation("Attempting to upload image for new food item: {FileName}", request.ImageFile.FileName);
                var uploadResult = await _cloudinaryService.UploadImageAsync(request.ImageFile, "food_items"); // Lưu vào folder 'food_items'

                if (!uploadResult.IsSuccess)
                {
                    _logger.LogWarning("Cloudinary image upload failed: {ErrorMessage}", uploadResult.ErrorMessage);
                    // Quyết định xem có nên tiếp tục tạo item không ảnh hay báo lỗi
                    // Ở đây ta chọn báo lỗi
                    return Result<FoodItemDto>.Failure($"Image upload failed: {uploadResult.ErrorMessage}", StatusCodes.Status400BadRequest);
                }
                imageUrl = uploadResult.Url;
                imagePublicId = uploadResult.PublicId; // Lưu lại Public ID
                _logger.LogInformation("Image uploaded successfully. URL: {ImageUrl}, PublicID: {PublicId}", imageUrl, imagePublicId);
            }

            // 2. Tạo FoodItem entity
            var foodItem = FoodItem.Create(
                request.Name,
                request.CaloriesPerServing,
                request.ServingSizeDescription,
                request.Category,
                request.ProteinGrams,
                request.CarbGrams,
                request.FatGrams,
                imageUrl // Truyền URL ảnh đã upload (hoặc null)
            );

            // Lưu thêm Public ID (Cần thêm trường này vào FoodItem entity và configuration)
            // Giả sử bạn đã thêm trường `ImagePublicId` vào FoodItem:
            foodItem.SetImagePublicId(imagePublicId); // Tạo phương thức này trong FoodItem

            try
            {
                // 3. Thêm vào DB
                await _unitOfWork.FoodItems.AddAsync(foodItem, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Food item '{FoodItemName}' created successfully with ID {FoodItemId}.", foodItem.Name, foodItem.FoodId);

                // 4. Map sang DTO và trả về
                var foodItemDto = _mapper.Map<FoodItemDto>(foodItem);
                return Result<FoodItemDto>.Success(foodItemDto, StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating food item '{FoodItemName}'.", request.Name);
                // Nếu lỗi xảy ra sau khi upload ảnh, cân nhắc xóa ảnh đã upload trên Cloudinary
                if (!string.IsNullOrEmpty(imagePublicId))
                {
                    _logger.LogWarning("Attempting to delete uploaded image {PublicId} due to DB error.", imagePublicId);
                    await _cloudinaryService.DeleteImageAsync(imagePublicId);
                }
                return Result<FoodItemDto>.Failure($"An error occurred while creating the food item: {ex.Message}", StatusCodes.Status500InternalServerError);
            }
        }
    }
    }
