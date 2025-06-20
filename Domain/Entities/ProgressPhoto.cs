using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    // FitnessApp.Domain/Entities/ProgressPhoto.cs
  

    public class ProgressPhoto : AuditableEntity
    {
        public int PhotoId { get; private set; }
        public int UserId { get; private set; }
        public DateOnly Date { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty; // URL từ Cloudinary
        public string? ImagePublicId { get; private set; } // Public ID của Cloudinary để xóa
        public PhotoFacing Facing { get; private set; }
        public string? Notes { get; private set; }

        public virtual User User { get; private set; } = null!; // Navigation property

        // Private constructor cho EF Core
        private ProgressPhoto() { }

        public static ProgressPhoto Create(
            int userId,
            DateOnly date,
            string imageUrl,
            string? imagePublicId,
            PhotoFacing facing,
            string? notes)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty.", nameof(imageUrl));
            if (date > DateOnly.FromDateTime(DateTime.Today.AddDays(1))) // Không cho phép ngày tương lai quá xa
                throw new ArgumentOutOfRangeException(nameof(date), "Photo date cannot be too far in the future.");

            return new ProgressPhoto
            {
                UserId = userId,
                Date = date,
                ImageUrl = imageUrl,
                ImagePublicId = imagePublicId,
                Facing = facing,
                Notes = notes
                // CreatedAt/By sẽ được set bởi Interceptor
            };
        }

        public void UpdateDetails(DateOnly date, PhotoFacing facing, string? notes)
        {
            if (date > DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                throw new ArgumentOutOfRangeException(nameof(date), "Photo date cannot be too far in the future.");

            Date = date;
            Facing = facing;
            Notes = notes;
            // UpdatedAt/By sẽ được set bởi Interceptor
        }

        public void UpdateImage(string newImageUrl, string? newImagePublicId)
        {
            if (string.IsNullOrWhiteSpace(newImageUrl))
                throw new ArgumentException("New image URL cannot be empty.", nameof(newImageUrl));

            ImageUrl = newImageUrl;
            ImagePublicId = newImagePublicId;
            // UpdatedAt/By sẽ được set bởi Interceptor
        }

        public void ClearImageDetails()
        {
            ImageUrl = string.Empty; // Hoặc null nếu cột cho phép
            ImagePublicId = null;
        }
    }
}
