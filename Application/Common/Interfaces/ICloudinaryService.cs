using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Responses.Dtos;
namespace Application.Common.Interfaces
{
    public interface ICloudinaryService
    {
        /// <summary>
        /// Uploads an image file to Cloudinary.
        /// </summary>
        /// <param name="file">The image file to upload.</param>
        /// <param name="folderName">Optional folder name on Cloudinary.</param>
        /// <returns>Result containing upload details or error.</returns>
        Task<CloudinaryUploadResult> UploadImageAsync(IFormFile file, string? folderName = null);

        /// <summary>
        /// Deletes an image from Cloudinary using its Public ID.
        /// </summary>
        /// <param name="publicId">The Public ID of the image to delete.</param>
        /// <returns>True if deletion was successful or resource didn't exist, false otherwise.</returns>
        Task<bool> DeleteImageAsync(string publicId);
    }
}
