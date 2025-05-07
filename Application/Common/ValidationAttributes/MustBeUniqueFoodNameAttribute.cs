using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MustBeUniqueFoodNameAttribute : ValidationAttribute
    {
        public MustBeUniqueFoodNameAttribute()
            : base("A food item with the name '{0}' already exists.") { }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success; // Tên rỗng hoặc null là hợp lệ (Required xử lý việc bắt buộc)
            }

            var foodName = value.ToString();

            // --- Anti-pattern: Lấy service từ ValidationContext ---
            var unitOfWork = validationContext.GetService<IUnitOfWork>();
            if (unitOfWork == null)
            {
                // Không thể lấy service -> không thể validate -> coi như hợp lệ hoặc ném lỗi?
                // Tốt nhất là nên ghi log lỗi ở đây
                Console.Error.WriteLine("ERROR: Could not resolve IUnitOfWork in MustBeUniqueFoodNameAttribute.");
                // Trả về Success để không chặn request một cách vô lý, nhưng đây là rủi ro.
                return ValidationResult.Success;
                // Hoặc ném Exception (nhưng sẽ gây lỗi 500 nếu không bắt đúng cách)
                // throw new InvalidOperationException("Could not resolve IUnitOfWork for validation.");
            }
            // ----------------------------------------------------

            // Lấy ID của đối tượng đang được validate (nếu là update)
            // Để làm được điều này, DTO/Command/Model của bạn cần có thuộc tính ID
            object? instance = validationContext.ObjectInstance;
            int currentId = 0; // Mặc định là 0 (cho trường hợp Create)
            var idProperty = instance?.GetType().GetProperty("FoodId"); // Giả sử có thuộc tính FoodId
            if (idProperty != null && idProperty.PropertyType == typeof(int))
            {
                currentId = (int?)idProperty.GetValue(instance) ?? 0;
            }

            // Gọi phương thức kiểm tra từ Repository (phương thức này cần được tạo)
            // Lưu ý: Gọi phương thức Async từ phương thức Sync là không lý tưởng.
            // Đây là lý do FluentValidation tốt hơn cho việc này.
            var task = Task.Run(async () => await unitOfWork.FoodItems.IsNameUniqueAsync(foodName!, currentId)); // Thêm IsNameUniqueAsync vào Repo
            bool isUnique = task.GetAwaiter().GetResult(); // Chờ kết quả (block thread) - KHÔNG TỐT!

            if (!isUnique)
            {
                // Sử dụng giá trị làm tham số {0} trong thông báo lỗi
                return new ValidationResult(FormatErrorMessage(foodName!));
            }

            return ValidationResult.Success;

        }
    }

}
