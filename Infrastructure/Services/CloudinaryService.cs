using Application.Common.Interfaces;
using Application.Responses.Dtos;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Extensions;
namespace Infrastructure.Services
{

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IOptions<CloudinarySettings> config, ILogger<CloudinaryService> logger)
        {
            _logger = logger;
            var settings = config.Value ?? throw new ArgumentNullException(nameof(config), "Cloudinary settings are not configured.");

            if (string.IsNullOrEmpty(settings.CloudName) ||
                string.IsNullOrEmpty(settings.ApiKey) ||
                string.IsNullOrEmpty(settings.ApiSecret))
            {
                throw new ArgumentException("Cloudinary configuration (CloudName, ApiKey, ApiSecret) is incomplete.");
            }

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Use https
        }

        public async Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string? folderName = "food_items")
        {
            if (file == null || file.Length == 0)
            {
                return new CloudinaryUploadResult(false, null, null, "No file provided for upload.");
            }

            // Kiểm tra kích thước file (ví dụ: giới hạn 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return new CloudinaryUploadResult(false, null, null, "File size exceeds the 5MB limit.");
            }

            // Kiểm tra loại file (chỉ cho phép ảnh)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return new CloudinaryUploadResult(false, null, null, "Invalid file type. Only JPG, PNG, GIF are allowed.");
            }


            ImageUploadResult? uploadResult = null;
            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    // PublicId = "my_custom_id", // Có thể tạo ID tùy chỉnh (phải duy nhất)
                    UseFilename = true, // Sử dụng tên file gốc (Cloudinary sẽ làm nó duy nhất nếu trùng)
                    UniqueFilename = true, // Đảm bảo tên file cuối cùng là duy nhất
                    Overwrite = false, // Không ghi đè nếu Public ID đã tồn tại (nếu không set PublicId)
                    Folder = folderName, // Tổ chức ảnh vào thư mục
                                         // Transformation = new Transformation().Width(800).Height(600).Crop("limit") // Resize ảnh khi upload
                };

                _logger.LogInformation("Uploading image {FileName} to Cloudinary folder {Folder}", file.FileName, folderName);
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image {FileName} to Cloudinary", file.FileName);
                return new CloudinaryUploadResult(false, null, null, $"Cloudinary upload failed: {ex.Message}");
            }


            if (uploadResult?.StatusCode == System.Net.HttpStatusCode.OK && uploadResult.SecureUrl != null)
            {
                _logger.LogInformation("Image {FileName} uploaded successfully. Public ID: {PublicId}, URL: {Url}",
                                    file.FileName, uploadResult.PublicId, uploadResult.SecureUrl.AbsoluteUri);
                return new CloudinaryUploadResult(true, uploadResult.PublicId, uploadResult.SecureUrl.AbsoluteUri, null);
            }
            else
            {
                var errorMessage = uploadResult?.Error?.Message ?? "Unknown error during Cloudinary upload.";
                _logger.LogError("Cloudinary upload failed for {FileName}. Status: {StatusCode}, Error: {Error}",
                                file.FileName, uploadResult?.StatusCode, errorMessage);
                return new CloudinaryUploadResult(false, null, null, $"Cloudinary upload failed: {errorMessage}");
            }
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
            {
                _logger.LogWarning("Attempted to delete image with empty or null public ID.");
                return true; // Coi như thành công vì không có gì để xóa
            }

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image // Chỉ định loại tài nguyên là ảnh
            };

            try
            {
                _logger.LogInformation("Deleting image with Public ID {PublicId} from Cloudinary", publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                // Result có thể là "ok", "not found", hoặc lỗi
                if (result.Result.ToLower() == "ok" || result.Result.ToLower() == "not found")
                {
                    _logger.LogInformation("Deletion result for Public ID {PublicId}: {Result}", publicId, result.Result);
                    return true;
                }
                else
                {
                    _logger.LogError("Cloudinary deletion failed for Public ID {PublicId}. Result: {Result}, Error: {Error}",
                                    publicId, result.Result, result.Error?.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image with Public ID {PublicId} from Cloudinary", publicId);
                return false;
            }
        }
    }
}