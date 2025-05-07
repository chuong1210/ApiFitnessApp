using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.ValidationAttributes
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EnsureFutureDateAttribute : ValidationAttribute
    {
        // Có thể thêm constructor để tùy chỉnh thông báo lỗi
        public EnsureFutureDateAttribute()
            : base("The date must be in the future.") // Thông báo lỗi mặc định
        { }

        // Override IsValid cơ bản (chỉ nhận giá trị)
        // public override bool IsValid(object? value)
        // {
        //     if (value is null) return true; // Null được coi là hợp lệ (dùng [Required] nếu muốn bắt buộc)
        //
        //     if (value is DateTime dateValue)
        //     {
        //         return dateValue > DateTime.Now;
        //     }
        //     // Xử lý DateOnly nếu cần
        //     if (value is DateOnly dateOnlyValue)
        //     {
        //          return dateOnlyValue > DateOnly.FromDateTime(DateTime.Today);
        //     }
        //     return false; // Kiểu dữ liệu không được hỗ trợ
        // }

        // Override IsValid với ValidationContext (mạnh mẽ hơn)
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // validationContext.MemberName: Tên thuộc tính đang validate ("ExpiryDate")
            // validationContext.DisplayName: Tên hiển thị (thường giống MemberName)
            // validationContext.ObjectInstance: Đối tượng chứa thuộc tính này (ví dụ: instance của FoodItemDto)

            if (value == null)
            {
                // Cho phép null, nếu bắt buộc phải có thì dùng thêm [Required]
                return ValidationResult.Success;
            }

            DateTime dateToCheck;

            if (value is DateTime dateTimeValue)
            {
                dateToCheck = dateTimeValue;
            }
            else if (value is DateOnly dateOnlyValue)
            {
                // Chuyển DateOnly thành DateTime để so sánh (chỉ cần so sánh ngày)
                dateToCheck = dateOnlyValue.ToDateTime(TimeOnly.MinValue);
                if (dateToCheck.Date <= DateTime.Today) // So sánh phần ngày
                {
                    // Sử dụng ErrorMessage đã được set (hoặc thông báo mặc định)
                    // Hoặc tạo thông báo lỗi mới với tên thuộc tính
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                return ValidationResult.Success; // Hợp lệ cho DateOnly
            }
            else
            {
                // Kiểu dữ liệu không phải DateTime hoặc DateOnly
                return new ValidationResult($"The field {validationContext.DisplayName} must be a valid date.");
            }


            // So sánh DateTime
            if (dateToCheck <= DateTime.Now)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            // Nếu mọi thứ ổn
            return ValidationResult.Success;
        }

        // (Tùy chọn) Override FormatErrorMessage để tùy chỉnh thông báo lỗi
        // public override string FormatErrorMessage(string name)
        // {
        //     return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        // }
    }
    }
